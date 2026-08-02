import 'package:flutter/material.dart';

import '../models/execucao_model.dart';
import '../models/planejamento_model.dart';
import '../services/execucao_service.dart';

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

  Future<void> _confirmarTomada() async {
    if (_registrando || _medicamentoTomado) return;

    setState(() {
      _registrando = true;
    });

    final execucao = ExecucaoModel.agora(
      idPlanejamento: planejamento.id,
    );

    final sucessoApi = await _execucaoService.registrar(execucao);

    if (!mounted) return;

    setState(() {
      _registrando = false;
      _medicamentoTomado = true;
    });

    final mensagem = sucessoApi
        ? 'Medicamento registrado com sucesso.'
        : 'Registro salvo no aparelho. Será necessário sincronizar com o servidor.';

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(mensagem),
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  void _abrirLeitor() {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Leitor de QR Code/NFC ainda não configurado.'),
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  String get _nomeMedicamento {
    // Confirme se o atributo no MedicamentoModel realmente se chama "nome".
    return planejamento.medicamento.nome;
  }

  String get _descricaoDosagem {
    final unidade = planejamento.unidadeDosagem.trim();

    if (unidade.isEmpty) {
      return planejamento.dosagem.toString();
    }

    return '${planejamento.dosagem} ${_formatarUnidade(unidade, planejamento.dosagem)}';
  }

  String _formatarUnidade(String unidade, int quantidade) {
    final valor = unidade.trim().toUpperCase();

    switch (valor) {
      case 'COMPRIMIDO':
        return quantidade == 1 ? 'comprimido' : 'comprimidos';

      case 'CAPSULA':
      case 'CÁPSULA':
        return quantidade == 1 ? 'cápsula' : 'cápsulas';

      case 'GOTA':
        return quantidade == 1 ? 'gota' : 'gotas';

      case 'ML':
        return 'mL';

      case 'MG':
        return 'mg';

      case 'G':
        return 'g';

      case 'UI':
        return 'UI';

      default:
        return unidade.toLowerCase();
    }
  }

  String get _instrucaoConsumo {
    switch (planejamento.instrucaoConsumo.toUpperCase()) {
      case 'JEJUM':
        return 'Tomar em jejum';

      case 'COM_ALIMENTO':
        return 'Tomar com alimento';

      case 'APOS_REFEICAO':
      case 'APÓS_REFEIÇÃO':
        return 'Tomar após a refeição';

      case 'ANTES_REFEICAO':
      case 'ANTES_DA_REFEICAO':
        return 'Tomar antes da refeição';

      default:
        return planejamento.instrucaoConsumo
            .replaceAll('_', ' ')
            .toLowerCase();
    }
  }

  IconData get _iconeHorario {
    final partes = planejamento.horario.split(':');
    final hora = int.tryParse(partes.first) ?? 0;

    if (hora >= 6 && hora < 12) {
      return Icons.wb_sunny_outlined;
    }

    if (hora >= 12 && hora < 18) {
      return Icons.light_mode_outlined;
    }

    return Icons.nightlight_round;
  }

  String get _periodoHorario {
    final partes = planejamento.horario.split(':');
    final hora = int.tryParse(partes.first) ?? 0;

    if (hora >= 6 && hora < 12) {
      return 'Manhã';
    }

    if (hora >= 12 && hora < 18) {
      return 'Tarde';
    }

    return 'Noite';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF1F5F9),
      appBar: AppBar(
        elevation: 0,
        centerTitle: true,
        backgroundColor: const Color(0xFF2563EB),
        foregroundColor: Colors.white,
        title: const Text(
          'Hora do medicamento',
          style: TextStyle(
            fontWeight: FontWeight.w700,
          ),
        ),
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.fromLTRB(20, 24, 20, 32),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              _MedicamentoCabecalho(
                nome: _nomeMedicamento,
                dosagem: _descricaoDosagem,
              ),
              const SizedBox(height: 20),
              _HorarioCard(
                horario: planejamento.horario,
                periodo: _periodoHorario,
                icone: _iconeHorario,
              ),
              const SizedBox(height: 20),
              _IlustracaoMedicamento(
                icone: _iconeHorario,
              ),
              const SizedBox(height: 20),
              if (_instrucaoConsumo.trim().isNotEmpty)
                _InstrucaoCard(
                  instrucao: _instrucaoConsumo,
                ),
              if (_instrucaoConsumo.trim().isNotEmpty)
                const SizedBox(height: 20),
              Row(
                children: [
                  Expanded(
                    child: _AcaoSecundaria(
                      icon: _somAtivado
                          ? Icons.volume_up_rounded
                          : Icons.volume_off_rounded,
                      label: _somAtivado ? 'Som ativado' : 'Som desativado',
                      onPressed: () {
                        setState(() {
                          _somAtivado = !_somAtivado;
                        });
                      },
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _AcaoSecundaria(
                      icon: Icons.qr_code_scanner_rounded,
                      label: 'Ler código',
                      onPressed: _abrirLeitor,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 28),
              SizedBox(
                height: 58,
                child: FilledButton.icon(
                  onPressed: _registrando || _medicamentoTomado
                      ? null
                      : _confirmarTomada,
                  style: FilledButton.styleFrom(
                    backgroundColor: const Color(0xFF22A957),
                    disabledBackgroundColor: _medicamentoTomado
                        ? const Color(0xFF86C99D)
                        : Colors.grey.shade400,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(18),
                    ),
                  ),
                  icon: _registrando
                      ? const SizedBox(
                          width: 23,
                          height: 23,
                          child: CircularProgressIndicator(
                            strokeWidth: 2.5,
                            color: Colors.white,
                          ),
                        )
                      : Icon(
                          _medicamentoTomado
                              ? Icons.check_circle
                              : Icons.thumb_up_alt_outlined,
                          size: 25,
                        ),
                  label: Text(
                    _registrando
                        ? 'Registrando...'
                        : _medicamentoTomado
                            ? 'Medicamento registrado'
                            : 'Já tomei',
                    style: const TextStyle(
                      fontSize: 17,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _MedicamentoCabecalho extends StatelessWidget {
  final String nome;
  final String dosagem;

  const _MedicamentoCabecalho({
    required this.nome,
    required this.dosagem,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [
            Color(0xFF2563EB),
            Color(0xFF3B82F6),
          ],
        ),
        borderRadius: BorderRadius.circular(24),
        boxShadow: const [
          BoxShadow(
            color: Color(0x332563EB),
            blurRadius: 18,
            offset: Offset(0, 8),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 66,
            height: 66,
            decoration: BoxDecoration(
              color: Colors.white.withValues(alpha: 0.18),
              shape: BoxShape.circle,
            ),
            child: const Icon(
              Icons.medication_rounded,
              color: Colors.white,
              size: 37,
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  nome,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 24,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 6),
                Row(
                  children: [
                    const Icon(
                      Icons.medication_liquid_rounded,
                      color: Colors.white,
                      size: 19,
                    ),
                    const SizedBox(width: 6),
                    Expanded(
                      child: Text(
                        dosagem,
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _HorarioCard extends StatelessWidget {
  final String horario;
  final String periodo;
  final IconData icone;

  const _HorarioCard({
    required this.horario,
    required this.periodo,
    required this.icone,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: 20,
        vertical: 18,
      ),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
        border: Border.all(
          color: const Color(0xFFE2E8F0),
        ),
      ),
      child: Row(
        children: [
          Container(
            width: 54,
            height: 54,
            decoration: const BoxDecoration(
              color: Color(0xFFEFF6FF),
              shape: BoxShape.circle,
            ),
            child: Icon(
              icone,
              color: const Color(0xFF2563EB),
              size: 29,
            ),
          ),
          const SizedBox(width: 15),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Horário da medicação',
                  style: TextStyle(
                    color: Color(0xFF64748B),
                    fontSize: 14,
                  ),
                ),
                const SizedBox(height: 3),
                Text(
                  periodo,
                  style: const TextStyle(
                    color: Color(0xFF334155),
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ),
          Text(
            horario,
            style: const TextStyle(
              color: Color(0xFF0F172A),
              fontSize: 29,
              fontWeight: FontWeight.w800,
            ),
          ),
        ],
      ),
    );
  }
}

class _IlustracaoMedicamento extends StatelessWidget {
  final IconData icone;

  const _IlustracaoMedicamento({
    required this.icone,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 220,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(26),
        border: Border.all(
          color: const Color(0xFFE2E8F0),
        ),
      ),
      child: Center(
        child: Container(
          width: 138,
          height: 138,
          decoration: const BoxDecoration(
            color: Color(0xFFEFF6FF),
            shape: BoxShape.circle,
          ),
          child: Icon(
            icone,
            size: 82,
            color: const Color(0xFF2563EB),
          ),
        ),
      ),
    );
  }
}

class _InstrucaoCard extends StatelessWidget {
  final String instrucao;

  const _InstrucaoCard({
    required this.instrucao,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: const Color(0xFFFFFBEB),
        borderRadius: BorderRadius.circular(18),
        border: Border.all(
          color: const Color(0xFFFDE68A),
        ),
      ),
      child: Row(
        children: [
          const Icon(
            Icons.restaurant_rounded,
            color: Color(0xFFD97706),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              instrucao,
              style: const TextStyle(
                color: Color(0xFF92400E),
                fontSize: 15,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _AcaoSecundaria extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onPressed;

  const _AcaoSecundaria({
    required this.icon,
    required this.label,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 92,
      child: OutlinedButton(
        onPressed: onPressed,
        style: OutlinedButton.styleFrom(
          foregroundColor: const Color(0xFF2563EB),
          backgroundColor: Colors.white,
          side: const BorderSide(
            color: Color(0xFFBFDBFE),
          ),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20),
          ),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              icon,
              size: 31,
            ),
            const SizedBox(height: 7),
            Text(
              label,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontWeight: FontWeight.w700,
              ),
            ),
          ],
        ),
      ),
    );
  }
}