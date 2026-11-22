import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import '../config/app_colors.dart';
import '../config/app_mocks.dart';
import '../services/sqflite_service.dart';

class AguardaMedicamentoView extends StatefulWidget {
  final VoidCallback onSimularSincronizacao;

  const AguardaMedicamentoView({
    super.key,
    required this.onSimularSincronizacao,
  });

  @override
  State<AguardaMedicamentoView> createState() => _AguardaMedicamentoViewState();
}

class _AguardaMedicamentoViewState extends State<AguardaMedicamentoView> {
  int _cliquesMedica = 0;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      height: double.infinity,
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.centerLeft,
          end: Alignment.centerRight,
          colors: [Color(0xFF1E78EB), Color(0xFF3E35E1)],
        ),
      ),
      child: SafeArea(
        bottom: false,
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 24),
              child: Center(
                child: GestureDetector(
                  onTap: _aoClicarTituloMedica,
                  behavior: HitTestBehavior.opaque,
                  child: const Text(
                    'Medica',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 32,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 1.0,
                    ),
                  ),
                ),
              ),
            ),
            Expanded(
              child: Container(
                width: double.infinity,
                decoration: const BoxDecoration(
                  color: Color(0xFFE5E5E5),
                  borderRadius: BorderRadius.vertical(top: Radius.circular(28)),
                ),
                child: SafeArea(
                  top: false,
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Container(
                      width: double.infinity,
                      height: double.infinity,
                      padding: const EdgeInsets.symmetric(
                        horizontal: 24,
                        vertical: 28,
                      ),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(18),
                        border: Border.all(
                          color: const Color(0xFFB5B5B5),
                          width: 2.2,
                        ),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withValues(alpha: 0.10),
                            blurRadius: 8,
                            offset: const Offset(0, 3),
                          ),
                        ],
                      ),
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Spacer(),
                          const Text(
                            HomeMock.mensagemAguardando,
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              color: Color(0xFF1E293B),
                              fontSize: 24,
                              fontWeight: FontWeight.w700,
                              height: 1.3,
                            ),
                          ),
                          const SizedBox(height: 18),
                          const _TresPontinhosAnimados(),
                          const Spacer(),
                          if (kDebugMode)
                            TextButton.icon(
                              onPressed: widget.onSimularSincronizacao,
                              icon: const Icon(Icons.sync_rounded, size: 20),
                              label: const Text(
                                'Simular chegada de planejamento (Alarme em 5s)',
                              ),
                              style: TextButton.styleFrom(
                                foregroundColor: AppColors.textMuted,
                              ),
                            ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _aoClicarTituloMedica() async {
    _cliquesMedica++;
    if (_cliquesMedica < 20) return;

    _cliquesMedica = 0;
    final planejamentos = await SqfliteService().listarPlanejamentos();
    if (!mounted) return;

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (dialogContext) {
        return AlertDialog(
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          titlePadding: const EdgeInsets.only(top: 6, right: 6),
          title: Align(
            alignment: Alignment.topRight,
            child: IconButton(
              icon: const Icon(Icons.close_rounded),
              onPressed: () => Navigator.of(dialogContext).pop(),
            ),
          ),
          content: Text(
            '${planejamentos.length} planejamentos salvos',
            textAlign: TextAlign.center,
            style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
          ),
          actionsAlignment: MainAxisAlignment.center,
          actions: [
            TextButton(
              onPressed: () => Navigator.of(dialogContext).pop(),
              child: const Text(
                'OK',
                style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              ),
            ),
          ],
        );
      },
    );
  }
}

class _TresPontinhosAnimados extends StatefulWidget {
  const _TresPontinhosAnimados();

  @override
  State<_TresPontinhosAnimados> createState() => _TresPontinhosAnimadosState();
}

class _TresPontinhosAnimadosState extends State<_TresPontinhosAnimados>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1200),
    )..repeat();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _controller,
      builder: (context, _) {
        return Row(
          mainAxisSize: MainAxisSize.min,
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(3, (index) {
            final start = index * 0.2;
            final end = start + 0.5;
            final t = Curves.easeInOut.transform(
              ((_controller.value - start) / (end - start)).clamp(0.0, 1.0),
            );
            final dy = -8.0 * (1.0 - (2.0 * t - 1.0).abs());
            final opacity = 0.35 + 0.65 * (1.0 - (2.0 * t - 1.0).abs());

            return Transform.translate(
              offset: Offset(0, dy),
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 5),
                child: Opacity(
                  opacity: opacity,
                  child: Container(
                    width: 14,
                    height: 14,
                    decoration: const BoxDecoration(
                      color: AppColors.primary,
                      shape: BoxShape.circle,
                    ),
                  ),
                ),
              ),
            );
          }),
        );
      },
    );
  }
}
