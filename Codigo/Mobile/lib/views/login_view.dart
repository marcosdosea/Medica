import 'package:flutter/material.dart';
import '../config/api_config.dart';
import '../config/app_colors.dart';
import '../services/fcm_service.dart';
import '../viewmodels/login_viewmodel.dart';
import 'placeholder_view.dart';
import 'qr_scanner_view.dart';

class LoginView extends StatefulWidget {
  const LoginView({super.key});

  @override
  State<LoginView> createState() => _LoginViewState();
}

class _LoginViewState extends State<LoginView> {
  late final LoginViewModel _viewModel;

  @override
  void initState() {
    super.initState();
    _viewModel = LoginViewModel();
    ApiConfig.inicializarConfiguracoes();
    FcmService.inicializar();
  }

  @override
  void dispose() {
    _viewModel.dispose();
    super.dispose();
  }

  Future<void> _processarToken(String token) async {
    final sucesso = await _viewModel.autenticar(token);

    if (!mounted) return;

    if (sucesso) {
      Navigator.of(context).pushReplacement(
        MaterialPageRoute(builder: (_) => const PlaceholderView()),
      );
    } else if (_viewModel.errorMessage != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            _viewModel.errorMessage!,
            textAlign: TextAlign.center,
            style: const TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w500,
            ),
          ),
          backgroundColor: AppColors.error,
          behavior: SnackBarBehavior.floating,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          margin: const EdgeInsets.all(16),
        ),
      );
    }
  }

  Future<void> _escanearQr() async {
    final token = await Navigator.of(context).push<String>(
      MaterialPageRoute(builder: (_) => const QrScannerView()),
    );

    if (token != null && token.isNotEmpty) {
      await _processarToken(token);
    }
  }


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          Container(
            decoration: const BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  AppColors.primary,
                  AppColors.primaryVariant,
                  AppColors.primaryDark,
                ],
              ),
            ),
          ),
          CustomPaint(
            size: Size.infinite,
            painter: _BackgroundWavePainter(),
          ),
          SafeArea(
            child: Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: ListenableBuilder(
                  listenable: _viewModel,
                  builder: (context, _) {
                    if (_viewModel.isLoading) {
                      return const Center(
                        child: CircularProgressIndicator(
                          color: Colors.white,
                        ),
                      );
                    }

                    return Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const SizedBox(height: 20),
                        Container(
                          width: 130,
                          height: 130,
                          decoration: BoxDecoration(
                            color: Colors.white,
                            shape: BoxShape.circle,
                            boxShadow: [
                              BoxShadow(
                                color: Colors.black.withValues(alpha: 0.15),
                                blurRadius: 20,
                                offset: const Offset(0, 8),
                              ),
                            ],
                          ),
                          child: ClipOval(
                            child: Padding(
                              padding: const EdgeInsets.all(18),
                              child: Image.asset(
                                'assets/imgs/logo_splash.png',
                                fit: BoxFit.contain,
                              ),
                            ),
                          ),
                        ),
                        const SizedBox(height: 48),
                        Row(
                          children: [
                            Expanded(
                              child: Container(
                                height: 1.5,
                                color: Colors.white60,
                              ),
                            ),
                            const Padding(
                              padding: EdgeInsets.symmetric(horizontal: 10),
                              child: Text(
                                'Como deseja entrar?',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontSize: 20,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ),
                            Expanded(
                              child: Container(
                                height: 1.5,
                                color: Colors.white60,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 40),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            _ActionItem(
                              label: 'QR Code',
                              icon: Icons.qr_code_2_rounded,
                              onTap: _escanearQr,
                            ),
                            const SizedBox(width: 44),
                            const _ActionItem(
                              label: 'PIN',
                              icon: Icons.dialpad_rounded,
                              enabled: false,
                            ),
                          ],
                        ),
                        const SizedBox(height: 40),
                      ],
                    );
                  },
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _ActionItem extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback? onTap;
  final bool enabled;

  const _ActionItem({
    required this.label,
    required this.icon,
    this.onTap,
    this.enabled = true,
  });

  @override
  Widget build(BuildContext context) {
    final bool isInteractive = enabled && onTap != null;

    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          label,
          style: TextStyle(
            color: isInteractive ? Colors.white : Colors.white38,
            fontSize: 18,
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 14),
        Material(
          color: isInteractive ? Colors.white : Colors.white.withValues(alpha: 0.3),
          shape: const CircleBorder(),
          elevation: isInteractive ? 6 : 0,
          shadowColor: Colors.black38,
          child: InkWell(
            customBorder: const CircleBorder(),
            onTap: isInteractive ? onTap : null,
            child: Container(
              width: 76,
              height: 76,
              alignment: Alignment.center,
              child: Icon(
                icon,
                color: isInteractive
                    ? AppColors.primaryDark
                    : AppColors.primaryDark.withValues(alpha: 0.35),
                size: 38,
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _BackgroundWavePainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.white.withValues(alpha: 0.05)
      ..style = PaintingStyle.fill;

    final pathTop = Path()
      ..moveTo(size.width, 0)
      ..lineTo(size.width * 0.35, 0)
      ..quadraticBezierTo(
        size.width * 0.65,
        size.height * 0.22,
        size.width,
        size.height * 0.28,
      )
      ..close();
    canvas.drawPath(pathTop, paint);

    final pathBottom = Path()
      ..moveTo(0, size.height)
      ..lineTo(0, size.height * 0.72)
      ..quadraticBezierTo(
        size.width * 0.45,
        size.height * 0.78,
        size.width * 0.65,
        size.height,
      )
      ..close();
    canvas.drawPath(pathBottom, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
