import 'package:flutter/material.dart';
import '../config/api_config.dart';
import '../config/app_colors.dart';
import '../config/session_manager.dart';
import 'login_view.dart';
import 'placeholder_view.dart';

class SplashView extends StatefulWidget {
  const SplashView({super.key});

  @override
  State<SplashView> createState() => _SplashViewState();
}

class _SplashViewState extends State<SplashView>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;
  late final Animation<double> _scaleAnimation;
  late final Animation<double> _textAnimation;
  late final Animation<double> _fadeAnimation;

  static const TextStyle _brandStyle = TextStyle(
    color: Colors.white,
    fontSize: 42,
    fontWeight: FontWeight.bold,
    letterSpacing: 1.2,
  );

  double _maxTextWidth = 185.0;

  @override
  void initState() {
    super.initState();

    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1600),
    );

    _scaleAnimation = Tween<double>(begin: 2.2, end: 1.0).animate(
      CurvedAnimation(
        parent: _controller,
        curve: const Interval(0.00, 0.45, curve: Curves.easeInOut),
      ),
    );

    _textAnimation = CurvedAnimation(
      parent: _controller,
      curve: const Interval(0.45, 0.85, curve: Curves.easeInOutCubic),
    );

    _fadeAnimation = CurvedAnimation(
      parent: _controller,
      curve: const Interval(0.50, 0.85, curve: Curves.easeIn),
    );

    WidgetsBinding.instance.addPostFrameCallback((_) => _iniciar());
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    precacheImage(const AssetImage('assets/imgs/logo_splash.png'), context);

    final textScaler = MediaQuery.textScalerOf(context);
    final textPainter = TextPainter(
      text: const TextSpan(text: 'Medica', style: _brandStyle),
      textDirection: TextDirection.ltr,
      textScaler: textScaler,
    )..layout();

    _maxTextWidth = textPainter.width + 16.0;
  }

  Future<void> _iniciar() async {
    if (!mounted) return;
    await Future.delayed(const Duration(milliseconds: 150));
    if (!mounted) return;

    await _controller.forward(from: 0.0);

    final dependencias = ApiConfig.inicializarConfiguracoes();
    final login = SessionManager.isLoggedIn();

    await dependencias;
    final logado = await login;

    if (!mounted) return;
    Navigator.of(context).pushReplacement(
      PageRouteBuilder(
        pageBuilder: (context, _, _) =>
            logado ? const PlaceholderView() : const LoginView(),
        transitionsBuilder: (context, animation, _, child) =>
            FadeTransition(opacity: animation, child: child),
        transitionDuration: const Duration(milliseconds: 350),
      ),
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
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
        child: Center(
          child: AnimatedBuilder(
            animation: _controller,
            builder: (context, _) {
              return Row(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  Transform.scale(
                    scale: _scaleAnimation.value,
                    alignment: Alignment.center,
                    child: Container(
                      width: 90,
                      height: 90,
                      decoration: BoxDecoration(
                        color: Colors.white,
                        shape: BoxShape.circle,
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withValues(alpha: 0.18),
                            blurRadius: 20,
                            offset: const Offset(0, 6),
                          ),
                        ],
                      ),
                      child: ClipOval(
                        child: Padding(
                          padding: const EdgeInsets.all(14),
                          child: Image.asset(
                            'assets/imgs/logo_splash.png',
                            fit: BoxFit.contain,
                          ),
                        ),
                      ),
                    ),
                  ),
                  ClipRect(
                    child: SizedBox(
                      width: _maxTextWidth * _textAnimation.value,
                      height: 90,
                      child: OverflowBox(
                        alignment: Alignment.centerLeft,
                        maxWidth: 400,
                        minWidth: 0,
                        maxHeight: 90,
                        minHeight: 90,
                        child: Opacity(
                          opacity: _fadeAnimation.value,
                          child: const Row(
                            mainAxisSize: MainAxisSize.min,
                            crossAxisAlignment: CrossAxisAlignment.center,
                            children: [
                              SizedBox(width: 16),
                              Text(
                                'Medica',
                                style: _brandStyle,
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                ],
              );
            },
          ),
        ),
      ),
    );
  }
}