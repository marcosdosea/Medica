import 'package:flutter/material.dart';
import '../config/app_mocks.dart';
import '../viewmodels/notificacao_viewmodel.dart';
import 'aguarda_medicamento_view.dart';

class HomeView extends StatefulWidget {
  const HomeView({super.key});

  @override
  State<HomeView> createState() => _HomeViewState();
}

class _HomeViewState extends State<HomeView> {
  late final NotificacaoViewModel _viewModel;

  @override
  void initState() {
    super.initState();
    _viewModel = NotificacaoViewModel();
    _viewModel.inicializar();
  }

  @override
  void dispose() {
    _viewModel.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFE5E5E5),
      body: ListenableBuilder(
        listenable: _viewModel,
        builder: (context, _) {
          return AnimatedSwitcher(
            duration: const Duration(milliseconds: 300),
            child: _viewModel.alarmeAtivo
                ? _TelaAlarmeAtivo(
                    key: const ValueKey('alarme'),
                    somMutado: _viewModel.somMutado,
                    onSilenciar: _viewModel.silenciarAlarme,
                    onConfirmar: _viewModel.confirmarMedicamentoTomado,
                  )
                : AguardaMedicamentoView(
                    key: const ValueKey('aguardando'),
                    onSimularSincronizacao:
                        _viewModel.simularChegadaPlanejamento,
                  ),
          );
        },
      ),
    );
  }
}

class _TelaAlarmeAtivo extends StatelessWidget {
  final bool somMutado;
  final VoidCallback onSilenciar;
  final VoidCallback onConfirmar;

  const _TelaAlarmeAtivo({
    super.key,
    required this.somMutado,
    required this.onSilenciar,
    required this.onConfirmar,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      height: double.infinity,
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.centerLeft,
          end: Alignment.centerRight,
          colors: [
            Color(0xFF1E78EB),
            Color(0xFF3E35E1),
          ],
        ),
      ),
      child: SafeArea(
        bottom: false,
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 10, 16, 12),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  Container(
                    width: 88,
                    height: 88,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                      border: Border.all(
                        color: Colors.black87,
                        width: 1.0,
                      ),
                    ),
                  ),
                  const SizedBox(width: 20),
                  const Expanded(
                    child: Text(
                      HomeMock.nomePaciente,
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 28,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ),
            ),
            Expanded(
              child: Stack(
                children: [
                  Positioned.fill(
                    top: 28,
                    child: Container(
                      decoration: const BoxDecoration(
                        color: Color(0xFFE5E5E5),
                        borderRadius: BorderRadius.vertical(
                          top: Radius.circular(28),
                        ),
                      ),
                    ),
                  ),
                  SafeArea(
                    top: false,
                    child: Padding(
                      padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
                      child: Column(
                        children: [
                          Container(
                            width: double.infinity,
                            padding: const EdgeInsets.symmetric(
                              horizontal: 16,
                              vertical: 8,
                            ),
                            decoration: BoxDecoration(
                              color: const Color(0xFF3482F6),
                              borderRadius: BorderRadius.circular(14),
                              border: Border.all(
                                color: Colors.black87,
                                width: 0.9,
                              ),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                              children: [
                                const Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Icon(
                                      Icons.access_time,
                                      color: Colors.white,
                                      size: 32,
                                    ),
                                    SizedBox(width: 10),
                                    Text(
                                      HomeMock.horario,
                                      style: TextStyle(
                                        color: Colors.white,
                                        fontSize: 34,
                                        fontWeight: FontWeight.w400,
                                      ),
                                    ),
                                  ],
                                ),
                                Container(
                                  width: 1.5,
                                  height: 40,
                                  color: Colors.white,
                                ),
                                Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    const SizedBox(
                                      width: 34,
                                      height: 30,
                                      child: CustomPaint(
                                        painter: _ComprimidoIconPainter(),
                                      ),
                                    ),
                                    const SizedBox(width: 12),
                                    Text(
                                      '${HomeMock.dosagem}',
                                      style: const TextStyle(
                                        color: Colors.white,
                                        fontSize: 34,
                                        fontWeight: FontWeight.w400,
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ),
                          const SizedBox(height: 10),
                          Expanded(
                            child: Container(
                              width: double.infinity,
                              padding: const EdgeInsets.fromLTRB(
                                20,
                                16,
                                20,
                                16,
                              ),
                              decoration: BoxDecoration(
                                color: Colors.white,
                                borderRadius: BorderRadius.circular(14),
                                border: Border.all(
                                  color: const Color(0xFFB5B5B5),
                                  width: 2.5,
                                ),
                                boxShadow: [
                                  BoxShadow(
                                    color: Colors.black.withValues(alpha: 0.12),
                                    blurRadius: 6,
                                    offset: const Offset(0, 3),
                                  ),
                                ],
                              ),
                              child: Column(
                                children: [
                                  Expanded(
                                    child: Center(
                                      child: Padding(
                                        padding: const EdgeInsets.all(12),
                                        child: Image.asset(
                                          HomeMock.imagemMedicamento,
                                          fit: BoxFit.contain,
                                        ),
                                      ),
                                    ),
                                  ),
                                  const SizedBox(height: 8),
                                  GestureDetector(
                                    onTap: onSilenciar,
                                    child: Container(
                                      width: 78,
                                      height: 62,
                                      decoration: BoxDecoration(
                                        color: const Color(0xFF3482F6),
                                        borderRadius: BorderRadius.circular(18),
                                        border: Border.all(
                                          color: Colors.black87,
                                          width: 0.9,
                                        ),
                                      ),
                                      child: Icon(
                                        somMutado
                                            ? Icons.volume_off_rounded
                                            : Icons.volume_up_rounded,
                                        color: Colors.white,
                                        size: 36,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                          const SizedBox(height: 14),
                          GestureDetector(
                            onTap: onConfirmar,
                            child: Container(
                              width: double.infinity,
                              height: 74,
                              decoration: BoxDecoration(
                                color: const Color(0xFF2FA346),
                                borderRadius: BorderRadius.circular(42),
                                boxShadow: [
                                  BoxShadow(
                                    color: Colors.black.withValues(alpha: 0.22),
                                    blurRadius: 8,
                                    offset: const Offset(0, 4),
                                  ),
                                ],
                              ),
                              child: const Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  Text(
                                    'Tomei',
                                    style: TextStyle(
                                      color: Colors.white,
                                      fontSize: 30,
                                      fontWeight: FontWeight.w400,
                                    ),
                                  ),
                                  SizedBox(width: 14),
                                  Icon(
                                    Icons.thumb_up_alt_outlined,
                                    color: Colors.white,
                                    size: 38,
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ComprimidoIconPainter extends CustomPainter {
  const _ComprimidoIconPainter();

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.white
      ..style = PaintingStyle.stroke
      ..strokeWidth = 1.8
      ..strokeCap = StrokeCap.round
      ..strokeJoin = StrokeJoin.round;

    final w = size.width;
    final h = size.height;
    final topOval = Rect.fromLTWH(2, 2, w - 4, h * 0.48);
    final bottomArc = Rect.fromLTWH(2, h * 0.42, w - 4, h * 0.48);

    canvas.drawOval(topOval, paint);
    canvas.drawLine(
      Offset(2, topOval.center.dy),
      Offset(2, bottomArc.center.dy),
      paint,
    );
    canvas.drawLine(
      Offset(w - 2, topOval.center.dy),
      Offset(w - 2, bottomArc.center.dy),
      paint,
    );
    canvas.drawArc(bottomArc, 0, 3.14159265, false, paint);
    canvas.drawLine(
      Offset(w * 0.22, topOval.top + topOval.height * 0.78),
      Offset(w * 0.78, topOval.top + topOval.height * 0.22),
      paint,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
