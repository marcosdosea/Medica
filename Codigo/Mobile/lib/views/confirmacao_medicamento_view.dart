import 'dart:async';
import 'package:audioplayers/audioplayers.dart';
import 'package:flutter/material.dart';

class ConfirmacaoMedicamentoView extends StatefulWidget {
  final VoidCallback? onConcluido;

  const ConfirmacaoMedicamentoView({
    super.key,
    this.onConcluido,
  });

  @override
  State<ConfirmacaoMedicamentoView> createState() =>
      _ConfirmacaoMedicamentoViewState();
}

class _ConfirmacaoMedicamentoViewState extends State<ConfirmacaoMedicamentoView>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;
  late final Animation<double> _scaleAnimation;
  final AudioPlayer _audioPlayer = AudioPlayer();
  StreamSubscription<void>? _playerCompleteSub;
  Timer? _timer;
  bool _finalizado = false;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 500),
    );

    _scaleAnimation = CurvedAnimation(
      parent: _controller,
      curve: Curves.elasticOut,
    );

    _playerCompleteSub = _audioPlayer.onPlayerComplete.listen((_) {
      _finalizar();
    });

    _controller.forward();
    _tocarSomConfirmacao();

    _timer = Timer(const Duration(milliseconds: 2200), _finalizar);
  }

  Future<void> _tocarSomConfirmacao() async {
    try {
      await _audioPlayer.play(AssetSource('sounds/confirmacao.mp3'));
    } catch (_) {}
  }

  void _finalizar() {
    if (_finalizado || !mounted) return;
    _finalizado = true;
    if (widget.onConcluido != null) {
      widget.onConcluido!();
    } else if (Navigator.of(context).canPop()) {
      Navigator.of(context).pop();
    }
  }

  @override
  void dispose() {
    _timer?.cancel();
    _playerCompleteSub?.cancel();
    _audioPlayer.dispose();
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: GestureDetector(
        onTap: _finalizar,
        behavior: HitTestBehavior.opaque,
        child: Container(
          width: double.infinity,
          height: double.infinity,
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [
                Color(0xFF2FA346),
                Color(0xFF1E8233),
              ],
            ),
          ),
          child: SafeArea(
            child: Center(
              child: ScaleTransition(
                scale: _scaleAnimation,
                child: Container(
                  width: 140,
                  height: 140,
                  decoration: BoxDecoration(
                    color: Colors.white,
                    shape: BoxShape.circle,
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withValues(alpha: 0.18),
                        blurRadius: 20,
                        offset: const Offset(0, 8),
                      ),
                    ],
                  ),
                  child: const Icon(
                    Icons.check_rounded,
                    color: Color(0xFF2FA346),
                    size: 92,
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
