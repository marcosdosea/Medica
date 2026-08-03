import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/material.dart';

import '../models/execucao_model.dart';
import '../models/planejamento_model.dart';
import '../services/execucao_service.dart';

/*
 * ATENÇÃO — PRÉ-REQUISITO PARA A IMAGEM CENTRAL APARECER CORRETAMENTE
 * =====================================================================
 * A imagem central (café da manhã, almoço, lanche, jantar, noite) é local,
 * ou seja, não vem do servidor. Ela precisa existir dentro do projeto em:
 *
 *   assets/images/instrucoes/cafe_manha.png
 *   assets/images/instrucoes/almoco.png
 *   assets/images/instrucoes/lanche.png
 *   assets/images/instrucoes/jantar.png
 *   assets/images/instrucoes/noite.png
 *
 * E o pubspec.yaml precisa declarar a pasta:
 *
 *   flutter:
 *     assets:
 *       - assets/images/instrucoes/
 *
 * Se algum desses PNGs não existir (ou o pubspec não tiver sido
 * atualizado + rodado `flutter pub get`), a tela cai automaticamente
 * no fallback (_ImagemInstrucaoPadrao) — que foi o motivo mais provável
 * de a tela aparecer "diferente" do esperado.
 */

class ExecucaoView extends StatefulWidget {
  final PlanejamentoModel planejamento;

  const ExecucaoView({
    super.key,
    required this.planejamento,
  });

  @override
  State<ExecucaoView> createState() => _ExecucaoViewState();
}

class _ExecucaoViewState extends State<ExecucaoView> {
  final ExecucaoService _execucaoService = ExecucaoService();

  bool _somAtivado = true;
  bool _registrando = false;
  bool _medicamentoTomado = false;

  PlanejamentoModel get planejamento => widget.planejamento;

  // =========================================================
  // DADOS RECEBIDOS DO SERVIDOR
  // =========================================================

  String get _nomeMedicamento {
    final nome = planejamento.medicamento.nome.trim();

    if (nome.isEmpty) {
      return 'Medicamento';
    }

    return nome;
  }

  String get _horarioFormatado {
    final horario = planejamento.horario.trim();

    if (horario.isEmpty) {
      return '--:--';
    }

    final partes = horario.split(':');

    if (partes.length < 2) {
      return horario;
    }

    final hora = partes[0].padLeft(2, '0');
    final minuto = partes[1].padLeft(2, '0');

    return '$hora:$minuto';
  }

  int get _horaPlanejamento {
    final horario = planejamento.horario.trim();

    if (horario.isEmpty) {
      return 0;
    }

    return int.tryParse(horario.split(':').first) ?? 0;
  }

  int get _quantidadeMedicamento {
    return planejamento.dosagem;
  }

  /// Ícone da dosagem, variando conforme a unidade cadastrada no servidor
  /// (comprimido, cápsula, gota, mL...). É isso que diferencia visualmente
  /// telas como a do Enalapril (comprimido) e a do Esomeprazol (cápsula).
  IconData get _iconeDosagem {
    final unidade = planejamento.unidadeDosagem.trim().toUpperCase();

    switch (unidade) {
      case 'CAPSULA':
      case 'CÁPSULA':
        return Icons.medication_rounded; // formato cápsula (comprido)

      case 'GOTA':
        return Icons.opacity_rounded;

      case 'ML':
        return Icons.water_drop_rounded;

      case 'COMPRIMIDO':
      default:
        return Icons.circle; // pastilha redonda
    }
  }

  String get _fotoMedicamento {
    /*
     * Caso o nome do campo do seu MedicamentoModel seja diferente,
     * altere somente esta linha.
     *
     * Exemplo esperado:
     * planejamento.medicamento.foto
     */
    return planejamento.medicamento.foto ?? '';
  }

  // =========================================================
  // IMAGEM LOCAL ESCOLHIDA PELO HORÁRIO
  // =========================================================

  String get _imagemInstrucao {
    final hora = _horaPlanejamento;

    /*
     * 05:00 até 09:59  -> café da manhã
     * 10:00 até 14:59  -> almoço
     * 15:00 até 17:59  -> lanche
     * 18:00 até 21:59  -> jantar
     * 22:00 até 04:59  -> noite
     */

    if (hora >= 5 && hora < 10) {
      return 'assets/images/instrucoes/cafe_manha.png';
    }

    if (hora >= 10 && hora < 15) {
      return 'assets/images/instrucoes/almoco.png';
    }

    if (hora >= 15 && hora < 18) {
      return 'assets/images/instrucoes/lanche.png';
    }

    if (hora >= 18 && hora < 22) {
      return 'assets/images/instrucoes/jantar.png';
    }

    return 'assets/images/instrucoes/noite.png';
  }

  // =========================================================
  // QUANTIDADE DE ÍCONES DE COMPRIMIDO
  // =========================================================

  int get _quantidadeIcones {
    if (_quantidadeMedicamento <= 0) {
      return 1;
    }

    /*
     * Limita a representação visual em 4 ícones para não quebrar
     * a interface. A dosagem real continua sendo a do servidor.
     */
    if (_quantidadeMedicamento > 4) {
      return 4;
    }

    return _quantidadeMedicamento;
  }

  // =========================================================
  // FOTO BASE64 DO SERVIDOR
  // =========================================================

  Uint8List? _converterBase64ParaBytes(String valor) {
    if (valor.trim().isEmpty) {
      return null;
    }

    try {
      var base64Limpo = valor.trim();

      /*
       * Aceita:
       * data:image/png;base64,AAA...
       * data:image/jpeg;base64,AAA...
       * AAA...
       */
      if (base64Limpo.contains(',')) {
        base64Limpo = base64Limpo.split(',').last;
      }

      base64Limpo = base64Limpo.replaceAll(
        RegExp(r'\s+'),
        '',
      );

      return base64Decode(base64Limpo);
    } catch (_) {
      return null;
    }
  }

  bool get _fotoEhUrl {
    final foto = _fotoMedicamento.toLowerCase();

    return foto.startsWith('http://') ||
        foto.startsWith('https://');
  }

  // =========================================================
  // AÇÕES
  // =========================================================

  Future<void> _confirmarTomada() async {
    if (_registrando || _medicamentoTomado) {
      return;
    }

    setState(() {
      _registrando = true;
    });

    try {
      final execucao = ExecucaoModel.agora(
        idPlanejamento: planejamento.id,
      );

      final sucessoApi = await _execucaoService.registrar(
        execucao,
      );

      if (!mounted) {
        return;
      }

      setState(() {
        _registrando = false;
        _medicamentoTomado = sucessoApi;
      });

      if (sucessoApi) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
              'Medicamento registrado com sucesso.',
            ),
            behavior: SnackBarBehavior.floating,
            backgroundColor: Color(0xFF16A34A),
          ),
        );

        await Future.delayed(
          const Duration(milliseconds: 700),
        );

        if (mounted) {
          Navigator.of(context).pop(true);
        }
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
              'Não foi possível registrar a medicação.',
            ),
            behavior: SnackBarBehavior.floating,
            backgroundColor: Color(0xFFDC2626),
          ),
        );
      }
    } catch (e) {
      if (!mounted) {
        return;
      }

      setState(() {
        _registrando = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Ocorreu um erro ao registrar a medicação.',
          ),
          behavior: SnackBarBehavior.floating,
          backgroundColor: Color(0xFFDC2626),
        ),
      );
    }
  }

  void _alternarSom() {
    setState(() {
      _somAtivado = !_somAtivado;
    });
  }

  void _abrirLeitor() {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text(
          'Leitor de código ainda não configurado.',
        ),
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  // =========================================================
  // TELA
  // =========================================================

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF3F4F6),
      body: SafeArea(
        child: LayoutBuilder(
          builder: (context, constraints) {
            return Center(
              child: ConstrainedBox(
                constraints: const BoxConstraints(
                  maxWidth: 480,
                ),
                child: Column(
                  children: [
                    Expanded(
                      child: SingleChildScrollView(
                        physics:
                            const AlwaysScrollableScrollPhysics(),
                        child: Column(
                          children: [
                            _construirCabecalho(),
                            _construirConteudoCentral(),
                          ],
                        ),
                      ),
                    ),
                    _construirBotaoConfirmacao(),
                  ],
                ),
              ),
            );
          },
        ),
      ),
    );
  }

  // =========================================================
  // CABEÇALHO AZUL
  // =========================================================

  Widget _construirCabecalho() {
    return Container(
      width: double.infinity,
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            Color(0xFF2563EB),
            Color(0xFF4338CA),
          ],
        ),
      ),
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(
              8,
              6,
              14,
              2,
            ),
            child: Row(
              children: [
                IconButton(
                  onPressed: () {
                    Navigator.of(context).pop();
                  },
                  tooltip: 'Voltar',
                  icon: const Icon(
                    Icons.arrow_back_ios_new_rounded,
                    color: Colors.white,
                    size: 22,
                  ),
                ),
                const Spacer(),
              ],
            ),
          ),

          Padding(
            padding: const EdgeInsets.fromLTRB(
              14,
              0,
              20,
              10,
            ),
            child: Row(
              children: [
                _FotoMedicamento(
                  foto: _fotoMedicamento,
                  fotoEhUrl: _fotoEhUrl,
                  converterBase64: _converterBase64ParaBytes,
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Text(
                    _nomeMedicamento,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 24,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ),
              ],
            ),
          ),

          _construirBarraHorario(),
        ],
      ),
    );
  }

  Widget _construirBarraHorario() {
    return Transform.translate(
      offset: const Offset(0, 8),
      child: Container(
        margin: const EdgeInsets.symmetric(
          horizontal: 10,
        ),
        padding: const EdgeInsets.symmetric(
          horizontal: 10,
          vertical: 7,
        ),
        decoration: BoxDecoration(
          color: const Color(0xFF3B82F6),
          borderRadius: BorderRadius.circular(10),
          border: Border.all(
            color: const Color(0xFF1D4ED8),
          ),
          boxShadow: const [
            BoxShadow(
              color: Color(0x33000000),
              blurRadius: 4,
              offset: Offset(0, 2),
            ),
          ],
        ),
        child: Row(
          children: [
            const Icon(
              Icons.access_time_rounded,
              color: Colors.white,
              size: 27,
            ),
            const SizedBox(width: 6),

            Text(
              _horarioFormatado,
              style: const TextStyle(
                color: Colors.white,
                fontSize: 30,
                height: 1,
                fontWeight: FontWeight.w400,
              ),
            ),

            const SizedBox(width: 10),

            Container(
              width: 1.5,
              height: 36,
              color: Colors.white70,
            ),

            const Spacer(),

            Flexible(
              child: _construirIconesDosagem(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _construirIconesDosagem() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.end,
      mainAxisSize: MainAxisSize.min,
      children: [
        ...List.generate(
          _quantidadeIcones,
          (index) {
            return Padding(
              padding: const EdgeInsets.only(left: 3),
              child: Icon(
                _iconeDosagem,
                color: Colors.white,
                size: _iconeDosagem == Icons.circle ? 18 : 30,
              ),
            );
          },
        ),

        if (_quantidadeMedicamento > 4)
          Padding(
            padding: const EdgeInsets.only(left: 4),
            child: Text(
              '+${_quantidadeMedicamento - 4}',
              style: const TextStyle(
                color: Colors.white,
                fontSize: 17,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
      ],
    );
  }

  // =========================================================
  // CONTEÚDO CENTRAL
  // =========================================================

  Widget _construirConteudoCentral() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        10,
        20,
        10,
        18,
      ),
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.fromLTRB(
          8,
          12,
          8,
          10,
        ),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(9),
          border: Border.all(
            color: const Color(0xFFB8B8B8),
            width: 2,
          ),
          boxShadow: const [
            BoxShadow(
              color: Color(0x33000000),
              blurRadius: 4,
              offset: Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          children: [
            _construirImagemInstrucao(),

            const SizedBox(height: 15),

            Row(
              children: [
                Expanded(
                  child: _BotaoAcao(
                    icon: _somAtivado
                        ? Icons.volume_up_rounded
                        : Icons.volume_off_rounded,
                    onPressed: _alternarSom,
                    tooltip: _somAtivado
                        ? 'Desativar som'
                        : 'Ativar som',
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: _BotaoAcao(
                    icon: Icons.qr_code_scanner_rounded,
                    onPressed: _abrirLeitor,
                    tooltip: 'Ler código',
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _construirImagemInstrucao() {
    return SizedBox(
      height: 175,
      width: double.infinity,
      child: Image.asset(
        _imagemInstrucao,
        fit: BoxFit.contain,
        errorBuilder: (
          context,
          error,
          stackTrace,
        ) {
          return const _ImagemInstrucaoPadrao();
        },
      ),
    );
  }

  // =========================================================
  // BOTÃO TOMEI
  // =========================================================

  Widget _construirBotaoConfirmacao() {
    return Container(
      color: const Color(0xFFF3F4F6),
      padding: const EdgeInsets.fromLTRB(
        10,
        8,
        10,
        12,
      ),
      child: SizedBox(
        width: double.infinity,
        height: 74,
        child: FilledButton(
          onPressed: _registrando || _medicamentoTomado
              ? null
              : _confirmarTomada,
          style: FilledButton.styleFrom(
            backgroundColor: const Color(0xFF22B455),
            disabledBackgroundColor:
                const Color(0xFF86C99D),
            foregroundColor: Colors.white,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(38),
            ),
            elevation: 5,
            shadowColor: Colors.black45,
          ),
          child: _registrando
              ? const Row(
                  mainAxisAlignment:
                      MainAxisAlignment.center,
                  children: [
                    SizedBox(
                      width: 26,
                      height: 26,
                      child: CircularProgressIndicator(
                        color: Colors.white,
                        strokeWidth: 3,
                      ),
                    ),
                    SizedBox(width: 14),
                    Text(
                      'Registrando...',
                      style: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                )
              : Row(
                  mainAxisAlignment:
                      MainAxisAlignment.center,
                  children: [
                    Text(
                      _medicamentoTomado
                          ? 'Registrado'
                          : 'Tomei',
                      style: const TextStyle(
                        fontSize: 28,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Icon(
                      _medicamentoTomado
                          ? Icons.check_circle_outline_rounded
                          : Icons.thumb_up_alt_outlined,
                      size: 39,
                    ),
                  ],
                ),
        ),
      ),
    );
  }
}

// ===========================================================
// FOTO DO MEDICAMENTO
// ===========================================================

class _FotoMedicamento extends StatelessWidget {
  final String foto;
  final bool fotoEhUrl;
  final Uint8List? Function(String valor) converterBase64;

  const _FotoMedicamento({
    required this.foto,
    required this.fotoEhUrl,
    required this.converterBase64,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 88,
      height: 88,
      decoration: BoxDecoration(
        color: const Color(0xFFF5C044),
        shape: BoxShape.circle,
        border: Border.all(
          color: const Color(0xFF263238),
          width: 1.2,
        ),
      ),
      clipBehavior: Clip.antiAlias,
      child: _construirImagem(),
    );
  }

  Widget _construirImagem() {
    if (foto.trim().isEmpty) {
      return const _FotoPadrao();
    }

    if (fotoEhUrl) {
      return Image.network(
        foto,
        fit: BoxFit.cover,
        errorBuilder: (
          context,
          error,
          stackTrace,
        ) {
          return const _FotoPadrao();
        },
        loadingBuilder: (
          context,
          child,
          loadingProgress,
        ) {
          if (loadingProgress == null) {
            return child;
          }

          return const Center(
            child: CircularProgressIndicator(
              strokeWidth: 2,
              color: Color(0xFF2563EB),
            ),
          );
        },
      );
    }

    final bytes = converterBase64(foto);

    if (bytes == null) {
      return const _FotoPadrao();
    }

    return Image.memory(
      bytes,
      fit: BoxFit.cover,
      gaplessPlayback: true,
      errorBuilder: (
        context,
        error,
        stackTrace,
      ) {
        return const _FotoPadrao();
      },
    );
  }
}

class _FotoPadrao extends StatelessWidget {
  const _FotoPadrao();

  @override
  Widget build(BuildContext context) {
    return const Center(
      child: Icon(
        Icons.medication_rounded,
        size: 52,
        color: Color(0xFF263238),
      ),
    );
  }
}

// ===========================================================
// BOTÕES AZUIS
// ===========================================================

class _BotaoAcao extends StatelessWidget {
  final IconData icon;
  final VoidCallback onPressed;
  final String tooltip;

  const _BotaoAcao({
    required this.icon,
    required this.onPressed,
    required this.tooltip,
  });

  @override
  Widget build(BuildContext context) {
    return Tooltip(
      message: tooltip,
      child: SizedBox(
        height: 88,
        child: FilledButton(
          onPressed: onPressed,
          style: FilledButton.styleFrom(
            backgroundColor: const Color(0xFF3B82F6),
            foregroundColor: Colors.white,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(18),
              side: const BorderSide(
                color: Color(0xFF1D4ED8),
              ),
            ),
            elevation: 2,
          ),
          child: Icon(
            icon,
            size: 43,
          ),
        ),
      ),
    );
  }
}

// ===========================================================
// FALLBACK DA IMAGEM CENTRAL
// ===========================================================

class _ImagemInstrucaoPadrao extends StatelessWidget {
  const _ImagemInstrucaoPadrao();

  /*
   * Usado somente se o PNG correspondente ao horário não for
   * encontrado em assets/images/instrucoes/. Reproduz a composição
   * das imagens de referência (ícone de refeição -> tempo -> prato)
   * usando apenas ícones do Flutter, para nunca deixar a tela "quebrada".
   */
  @override
  Widget build(BuildContext context) {
    return const Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: [
        Icon(
          Icons.free_breakfast_rounded,
          size: 56,
          color: Color(0xFF1F2937),
        ),
        Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              '30 min',
              style: TextStyle(
                color: Color(0xFF6B7280),
                fontSize: 20,
                fontWeight: FontWeight.w700,
              ),
            ),
            SizedBox(height: 2),
            Icon(
              Icons.arrow_forward_rounded,
              size: 34,
              color: Color(0xFF6B7280),
            ),
          ],
        ),
        Icon(
          Icons.lunch_dining_rounded,
          size: 56,
          color: Color(0xFF1F2937),
        ),
      ],
    );
  }
}