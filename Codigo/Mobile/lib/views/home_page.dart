import 'package:flutter/material.dart';

import '../models/planejamento_model.dart';
import '../services/planejamento_service.dart';
import 'execucao_view.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final PlanejamentoService _planejamentoService = PlanejamentoService();

  List<PlanejamentoModel> _planejamentos = [];

  bool _carregando = true;
  bool _atualizando = false;
  String? _erro;

  @override
  void initState() {
    super.initState();
    _carregarPlanejamentos();
  }

  Future<void> _carregarPlanejamentos({
    bool forcarAtualizacao = false,
  }) async {
    if (forcarAtualizacao) {
      setState(() {
        _atualizando = true;
        _erro = null;
      });
    } else {
      setState(() {
        _carregando = true;
        _erro = null;
      });
    }

    try {
      final planejamentos = await _planejamentoService.listar(
        forcarAtualizacao: forcarAtualizacao,
      );

      if (!mounted) return;

      final planejamentosDoDia = planejamentos
          .where((planejamento) {
            return planejamento.isAtivo && planejamento.isHoje;
          })
          .toList()
        ..sort((a, b) => a.horario.compareTo(b.horario));

      setState(() {
        _planejamentos = planejamentosDoDia;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _erro = 'Não foi possível carregar os medicamentos.';
      });
    } finally {
      if (!mounted) return;

      setState(() {
        _carregando = false;
        _atualizando = false;
      });
    }
  }

  Future<void> _abrirExecucao(
    PlanejamentoModel planejamento,
  ) async {
    await Navigator.of(context).push(
      MaterialPageRoute(
        builder: (_) => ExecucaoView(
          planejamento: planejamento,
        ),
      ),
    );

    // Ao voltar da tela de execução, atualiza os dados.
    await _carregarPlanejamentos(
      forcarAtualizacao: true,
    );
  }

  String _nomeMedicamento(PlanejamentoModel planejamento) {
    // Confirme se o atributo no MedicamentoModel se chama "nome".
    return planejamento.medicamento.nome;
  }

  String _formatarDosagem(PlanejamentoModel planejamento) {
    final quantidade = planejamento.dosagem;
    final unidade = planejamento.unidadeDosagem.trim();

    if (unidade.isEmpty) {
      return quantidade.toString();
    }

    final unidadeFormatada = _formatarUnidade(
      unidade,
      quantidade,
    );

    return '$quantidade $unidadeFormatada';
  }

  String _formatarUnidade(
    String unidade,
    int quantidade,
  ) {
    switch (unidade.toUpperCase()) {
      case 'COMPRIMIDO':
        return quantidade == 1
            ? 'comprimido'
            : 'comprimidos';

      case 'CAPSULA':
      case 'CÁPSULA':
        return quantidade == 1
            ? 'cápsula'
            : 'cápsulas';

      case 'GOTA':
        return quantidade == 1
            ? 'gota'
            : 'gotas';

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

  String _formatarInstrucao(String instrucao) {
    switch (instrucao.toUpperCase()) {
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
        if (instrucao.trim().isEmpty) {
          return 'Siga a orientação médica';
        }

        return instrucao
            .replaceAll('_', ' ')
            .toLowerCase();
    }
  }

  String _saudacao() {
    final hora = DateTime.now().hour;

    if (hora < 12) {
      return 'Bom dia';
    }

    if (hora < 18) {
      return 'Boa tarde';
    }

    return 'Boa noite';
  }

  String _dataAtual() {
    const meses = [
      'janeiro',
      'fevereiro',
      'março',
      'abril',
      'maio',
      'junho',
      'julho',
      'agosto',
      'setembro',
      'outubro',
      'novembro',
      'dezembro',
    ];

    final agora = DateTime.now();

    return '${agora.day} de ${meses[agora.month - 1]}';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF1F5F9),
      appBar: AppBar(
        backgroundColor: const Color(0xFF2563EB),
        foregroundColor: Colors.white,
        elevation: 0,
        titleSpacing: 20,
        title: const Row(
          children: [
            Icon(
              Icons.medication_rounded,
              size: 30,
            ),
            SizedBox(width: 10),
            Text(
              'Medica',
              style: TextStyle(
                fontWeight: FontWeight.w800,
              ),
            ),
          ],
        ),
        actions: [
          IconButton(
            onPressed: _atualizando
                ? null
                : () {
                    _carregarPlanejamentos(
                      forcarAtualizacao: true,
                    );
                  },
            tooltip: 'Atualizar',
            icon: _atualizando
                ? const SizedBox(
                    width: 21,
                    height: 21,
                    child: CircularProgressIndicator(
                      color: Colors.white,
                      strokeWidth: 2.5,
                    ),
                  )
                : const Icon(
                    Icons.refresh_rounded,
                  ),
          ),
          const SizedBox(width: 8),
        ],
      ),
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: () {
            return _carregarPlanejamentos(
              forcarAtualizacao: true,
            );
          },
          child: _construirConteudo(),
        ),
      ),
    );
  }

  Widget _construirConteudo() {
    if (_carregando) {
      return const _EstadoCarregando();
    }

    if (_erro != null && _planejamentos.isEmpty) {
      return _EstadoErro(
        mensagem: _erro!,
        onTentarNovamente: () {
          _carregarPlanejamentos(
            forcarAtualizacao: true,
          );
        },
      );
    }

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(
        20,
        22,
        20,
        32,
      ),
      children: [
        Text(
          _saudacao(),
          style: const TextStyle(
            color: Color(0xFF0F172A),
            fontSize: 28,
            fontWeight: FontWeight.w800,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          'Hoje, ${_dataAtual()}',
          style: const TextStyle(
            color: Color(0xFF64748B),
            fontSize: 16,
          ),
        ),
        const SizedBox(height: 24),
        _ResumoCard(
          quantidade: _planejamentos.length,
        ),
        const SizedBox(height: 26),
        const Text(
          'Medicamentos de hoje',
          style: TextStyle(
            color: Color(0xFF0F172A),
            fontSize: 20,
            fontWeight: FontWeight.w800,
          ),
        ),
        const SizedBox(height: 6),
        const Text(
          'Toque em um medicamento para visualizar e confirmar a tomada.',
          style: TextStyle(
            color: Color(0xFF64748B),
            fontSize: 14,
            height: 1.4,
          ),
        ),
        const SizedBox(height: 18),
        if (_planejamentos.isEmpty)
          const _EstadoVazio()
        else
          ..._planejamentos.map(
            (planejamento) {
              return Padding(
                padding: const EdgeInsets.only(
                  bottom: 14,
                ),
                child: _PlanejamentoCard(
                  nome: _nomeMedicamento(
                    planejamento,
                  ),
                  horario: planejamento.horario,
                  dosagem: _formatarDosagem(
                    planejamento,
                  ),
                  instrucao: _formatarInstrucao(
                    planejamento.instrucaoConsumo,
                  ),
                  onTap: () {
                    _abrirExecucao(
                      planejamento,
                    );
                  },
                ),
              );
            },
          ),
      ],
    );
  }
}

class _ResumoCard extends StatelessWidget {
  final int quantidade;

  const _ResumoCard({
    required this.quantidade,
  });

  @override
  Widget build(BuildContext context) {
    final texto = quantidade == 1
        ? '1 medicamento programado'
        : '$quantidade medicamentos programados';

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
            width: 62,
            height: 62,
            decoration: BoxDecoration(
              color: Colors.white.withValues(
                alpha: 0.18,
              ),
              shape: BoxShape.circle,
            ),
            child: const Icon(
              Icons.calendar_today_rounded,
              color: Colors.white,
              size: 30,
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment:
                  CrossAxisAlignment.start,
              children: [
                const Text(
                  'Sua medicação hoje',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 17,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 5),
                Text(
                  texto,
                  style: TextStyle(
                    color: Colors.white.withValues(
                      alpha: 0.9,
                    ),
                    fontSize: 15,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _PlanejamentoCard extends StatelessWidget {
  final String nome;
  final String horario;
  final String dosagem;
  final String instrucao;
  final VoidCallback onTap;

  const _PlanejamentoCard({
    required this.nome,
    required this.horario,
    required this.dosagem,
    required this.instrucao,
    required this.onTap,
  });

  int get _hora {
    return int.tryParse(
          horario.split(':').first,
        ) ??
        0;
  }

  IconData get _iconeHorario {
    if (_hora >= 6 && _hora < 12) {
      return Icons.wb_sunny_outlined;
    }

    if (_hora >= 12 && _hora < 18) {
      return Icons.light_mode_outlined;
    }

    return Icons.nightlight_round;
  }

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(22),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(22),
        child: Container(
          padding: const EdgeInsets.all(18),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(22),
            border: Border.all(
              color: const Color(0xFFE2E8F0),
            ),
          ),
          child: Row(
            crossAxisAlignment:
                CrossAxisAlignment.center,
            children: [
              Container(
                width: 58,
                height: 58,
                decoration: const BoxDecoration(
                  color: Color(0xFFEFF6FF),
                  shape: BoxShape.circle,
                ),
                child: const Icon(
                  Icons.medication_rounded,
                  color: Color(0xFF2563EB),
                  size: 32,
                ),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment:
                      CrossAxisAlignment.start,
                  children: [
                    Text(
                      nome,
                      maxLines: 2,
                      overflow:
                          TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: Color(0xFF0F172A),
                        fontSize: 18,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                    const SizedBox(height: 7),
                    Row(
                      children: [
                        const Icon(
                          Icons.medication_liquid_rounded,
                          size: 18,
                          color: Color(0xFF64748B),
                        ),
                        const SizedBox(width: 5),
                        Flexible(
                          child: Text(
                            dosagem,
                            overflow:
                                TextOverflow.ellipsis,
                            style: const TextStyle(
                              color: Color(0xFF475569),
                              fontSize: 14,
                              fontWeight:
                                  FontWeight.w600,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 5),
                    Text(
                      instrucao,
                      maxLines: 1,
                      overflow:
                          TextOverflow.ellipsis,
                      style: const TextStyle(
                        color: Color(0xFF64748B),
                        fontSize: 13,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 10),
              Column(
                crossAxisAlignment:
                    CrossAxisAlignment.end,
                children: [
                  Icon(
                    _iconeHorario,
                    color: const Color(0xFF2563EB),
                    size: 22,
                  ),
                  const SizedBox(height: 4),
                  Text(
                    horario,
                    style: const TextStyle(
                      color: Color(0xFF0F172A),
                      fontSize: 19,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  const SizedBox(height: 7),
                  const Icon(
                    Icons.chevron_right_rounded,
                    color: Color(0xFF94A3B8),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _EstadoCarregando extends StatelessWidget {
  const _EstadoCarregando();

  @override
  Widget build(BuildContext context) {
    return const Center(
      child: CircularProgressIndicator(),
    );
  }
}

class _EstadoVazio extends StatelessWidget {
  const _EstadoVazio();

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: 24,
        vertical: 38,
      ),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
        border: Border.all(
          color: const Color(0xFFE2E8F0),
        ),
      ),
      child: const Column(
        children: [
          Icon(
            Icons.event_available_rounded,
            color: Color(0xFF22C55E),
            size: 55,
          ),
          SizedBox(height: 14),
          Text(
            'Nenhum medicamento para hoje',
            textAlign: TextAlign.center,
            style: TextStyle(
              color: Color(0xFF0F172A),
              fontSize: 18,
              fontWeight: FontWeight.w800,
            ),
          ),
          SizedBox(height: 7),
          Text(
            'Não há planejamentos ativos para o dia atual.',
            textAlign: TextAlign.center,
            style: TextStyle(
              color: Color(0xFF64748B),
              height: 1.4,
            ),
          ),
        ],
      ),
    );
  }
}

class _EstadoErro extends StatelessWidget {
  final String mensagem;
  final VoidCallback onTentarNovamente;

  const _EstadoErro({
    required this.mensagem,
    required this.onTentarNovamente,
  });

  @override
  Widget build(BuildContext context) {
    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(24),
      children: [
        const SizedBox(height: 100),
        const Icon(
          Icons.cloud_off_rounded,
          size: 62,
          color: Color(0xFFEF4444),
        ),
        const SizedBox(height: 18),
        Text(
          mensagem,
          textAlign: TextAlign.center,
          style: const TextStyle(
            color: Color(0xFF0F172A),
            fontSize: 18,
            fontWeight: FontWeight.w700,
          ),
        ),
        const SizedBox(height: 20),
        Center(
          child: FilledButton.icon(
            onPressed: onTentarNovamente,
            icon: const Icon(
              Icons.refresh_rounded,
            ),
            label: const Text(
              'Tentar novamente',
            ),
          ),
        ),
      ],
    );
  }
}