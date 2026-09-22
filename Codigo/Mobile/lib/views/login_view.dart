import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:mobile_scanner/mobile_scanner.dart';

import '../services/auth_service.dart';
import 'home_page.dart';

/// Tela 00 — Login do dispositivo Medica.
///
/// O usuário pode se autenticar de duas formas:
/// - Escanear um QR Code gerado pelo sistema web
/// - Digitar manualmente o PIN de pareamento
///
/// Após autenticação bem-sucedida, navega para [HomePage] substituindo
/// esta rota (o usuário não pode voltar para o login).
class LoginView extends StatefulWidget {
  const LoginView({super.key});

  @override
  State<LoginView> createState() => _LoginViewState();
}

class _LoginViewState extends State<LoginView> {
  bool _carregando = false;

  // ─── Autenticação ───────────────────────────────────────────────────────────

  Future<void> _autenticar(String tokenPareamento) async {
    if (_carregando) return;

    setState(() => _carregando = true);

    final resultado = await AuthService.associarDispositivo(
      tokenPareamento: tokenPareamento,
    );

    if (!mounted) return;

    setState(() => _carregando = false);

    if (resultado.sucesso) {
      Navigator.of(context).pushReplacement(
        MaterialPageRoute(builder: (_) => const HomePage()),
      );
    } else {
      _mostrarErro(resultado.erro ?? 'Erro desconhecido.');
    }
  }

  void _mostrarErro(String mensagem) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(mensagem),
        backgroundColor: const Color(0xFFDC2626),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        margin: const EdgeInsets.all(16),
      ),
    );
  }

  // ─── QR Code ────────────────────────────────────────────────────────────────

  Future<void> _abrirQrScanner() async {
    final token = await Navigator.of(context).push<String>(
      MaterialPageRoute(builder: (_) => const _QrScannerPage()),
    );

    if (token != null && token.isNotEmpty) {
      await _autenticar(token);
    }
  }

  // ─── PIN ────────────────────────────────────────────────────────────────────

  Future<void> _abrirDigitarPin() async {
    final token = await showDialog<String>(
      context: context,
      builder: (_) => const _PinDialog(),
    );

    if (token != null && token.isNotEmpty) {
      await _autenticar(token);
    }
  }

  // ─── UI ─────────────────────────────────────────────────────────────────────

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF1A3A8F),
      body: Stack(
        children: [
          // Curvas decorativas de fundo
          const _FundoDecorado(),

          // Conteúdo principal
          SafeArea(
            child: Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 32),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const SizedBox(height: 40),

                    // ── Logo ──────────────────────────────────────────────────
                    _LogoMedica(),

                    const SizedBox(height: 48),

                    // ── Divisor "Como deseja entrar?" ─────────────────────────
                    _DivisorEntrada(),

                    const SizedBox(height: 28),

                    // ── Botão QR Code ─────────────────────────────────────────
                    _BotaoEntrada(
                      id: 'btn_qr_code',
                      texto: 'Escanear QR Code',
                      icone: Icons.qr_code_scanner_rounded,
                      carregando: _carregando,
                      onTap: _abrirQrScanner,
                    ),

                    const SizedBox(height: 14),

                    // ── Botão PIN ─────────────────────────────────────────────
                    _BotaoEntrada(
                      id: 'btn_digitar_pin',
                      texto: 'Digitar PIN',
                      icone: Icons.lock_rounded,
                      carregando: _carregando,
                      onTap: _abrirDigitarPin,
                    ),

                    const SizedBox(height: 40),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// COMPONENTES INTERNOS
// ═══════════════════════════════════════════════════════════════════════════════

/// Curvas decorativas no topo e na base da tela de login.
class _FundoDecorado extends StatelessWidget {
  const _FundoDecorado();

  @override
  Widget build(BuildContext context) {
    return SizedBox.expand(
      child: CustomPaint(painter: _FundoPainter()),
    );
  }
}

class _FundoPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paintTop = Paint()
      ..color = const Color(0xFF1565C0)
      ..style = PaintingStyle.fill;

    // Onda superior
    final pathTop = Path()
      ..moveTo(0, 0)
      ..lineTo(size.width, 0)
      ..lineTo(size.width, size.height * 0.30)
      ..quadraticBezierTo(
        size.width * 0.55,
        size.height * 0.18,
        0,
        size.height * 0.28,
      )
      ..close();
    canvas.drawPath(pathTop, paintTop);

    final paintBot = Paint()
      ..color = const Color(0xFF0D2B7A)
      ..style = PaintingStyle.fill;

    // Onda inferior
    final pathBot = Path()
      ..moveTo(0, size.height)
      ..lineTo(size.width, size.height)
      ..lineTo(size.width, size.height * 0.75)
      ..quadraticBezierTo(
        size.width * 0.45,
        size.height * 0.85,
        0,
        size.height * 0.78,
      )
      ..close();
    canvas.drawPath(pathBot, paintBot);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

/// Logo circular com ícone de coração e texto "Medica".
class _LogoMedica extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Container(
          width: 108,
          height: 108,
          decoration: BoxDecoration(
            color: const Color(0xFFD4D8F0),
            shape: BoxShape.circle,
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.3),
                blurRadius: 20,
                offset: const Offset(0, 8),
              ),
            ],
          ),
          child: const Icon(
            Icons.favorite_rounded,
            color: Color(0xFF5B6EBD),
            size: 60,
          ),
        ),
        const SizedBox(height: 18),
        const Text(
          'Medica',
          style: TextStyle(
            color: Colors.white,
            fontSize: 32,
            fontWeight: FontWeight.w700,
            letterSpacing: 1.0,
          ),
        ),
      ],
    );
  }
}

/// Linha divisória com texto "— Como deseja entrar? —"
class _DivisorEntrada extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Container(height: 1.5, color: Colors.white38),
        ),
        const Padding(
          padding: EdgeInsets.symmetric(horizontal: 12),
          child: Text(
            'Como deseja entrar?',
            style: TextStyle(
              color: Colors.white70,
              fontSize: 14,
              fontWeight: FontWeight.w500,
              letterSpacing: 0.4,
            ),
          ),
        ),
        Expanded(
          child: Container(height: 1.5, color: Colors.white38),
        ),
      ],
    );
  }
}

/// Botão de entrada estilizado (QR Code ou PIN).
class _BotaoEntrada extends StatelessWidget {
  final String id;
  final String texto;
  final IconData icone;
  final bool carregando;
  final VoidCallback onTap;

  const _BotaoEntrada({
    required this.id,
    required this.texto,
    required this.icone,
    required this.carregando,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      key: Key(id),
      color: Colors.white,
      borderRadius: BorderRadius.circular(10),
      child: InkWell(
        onTap: carregando ? null : onTap,
        borderRadius: BorderRadius.circular(10),
        child: Container(
          width: double.infinity,
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 20),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                texto,
                style: const TextStyle(
                  color: Color(0xFF1A1A2E),
                  fontSize: 17,
                  fontWeight: FontWeight.w500,
                ),
              ),
              Icon(
                icone,
                color: const Color(0xFF1A3A8F),
                size: 26,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// SCANNER DE QR CODE
// ═══════════════════════════════════════════════════════════════════════════════

class _QrScannerPage extends StatefulWidget {
  const _QrScannerPage();

  @override
  State<_QrScannerPage> createState() => _QrScannerPageState();
}

class _QrScannerPageState extends State<_QrScannerPage> {
  bool _lido = false;

  void _onDetect(BarcodeCapture capture) {
    if (_lido) return;

    final barcode = capture.barcodes.firstOrNull;
    final valor = barcode?.rawValue;

    if (valor != null && valor.isNotEmpty) {
      _lido = true;
      Navigator.of(context).pop(valor);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      appBar: AppBar(
        backgroundColor: const Color(0xFF1A3A8F),
        foregroundColor: Colors.white,
        title: const Text('Escanear QR Code'),
      ),
      body: Stack(
        children: [
          MobileScanner(onDetect: _onDetect),

          // Moldura de mira centralizada
          Center(
            child: Container(
              width: 240,
              height: 240,
              decoration: BoxDecoration(
                border: Border.all(color: Colors.white, width: 2.5),
                borderRadius: BorderRadius.circular(16),
              ),
            ),
          ),

          // Instrução
          Positioned(
            bottom: 60,
            left: 0,
            right: 0,
            child: Container(
              margin: const EdgeInsets.symmetric(horizontal: 32),
              padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 20),
              decoration: BoxDecoration(
                color: Colors.black54,
                borderRadius: BorderRadius.circular(12),
              ),
              child: const Text(
                'Aponte a câmera para o QR Code do paciente',
                textAlign: TextAlign.center,
                style: TextStyle(color: Colors.white, fontSize: 15),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// DIALOG DE PIN
// ═══════════════════════════════════════════════════════════════════════════════

class _PinDialog extends StatefulWidget {
  const _PinDialog();

  @override
  State<_PinDialog> createState() => _PinDialogState();
}

class _PinDialogState extends State<_PinDialog> {
  final _controller = TextEditingController();
  final _focusNode = FocusNode();
  bool _pinValido = false;

  @override
  void initState() {
    super.initState();
    _controller.addListener(() {
      setState(() {
        _pinValido = _controller.text.trim().isNotEmpty;
      });
    });

    // Abre o teclado automaticamente
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _focusNode.requestFocus();
    });
  }

  @override
  void dispose() {
    _controller.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(18)),
      title: const Row(
        children: [
          Icon(Icons.lock_rounded, color: Color(0xFF1A3A8F)),
          SizedBox(width: 10),
          Text(
            'Digitar PIN',
            style: TextStyle(
              fontWeight: FontWeight.w700,
              fontSize: 20,
            ),
          ),
        ],
      ),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Digite o código de pareamento fornecido pelo sistema:',
            style: TextStyle(color: Color(0xFF475569), fontSize: 14),
          ),
          const SizedBox(height: 16),
          TextField(
            key: const Key('campo_pin'),
            controller: _controller,
            focusNode: _focusNode,
            keyboardType: TextInputType.text,
            inputFormatters: [
              // Aceita letras e números (UUID ou PIN alfanumérico)
              FilteringTextInputFormatter.allow(RegExp(r'[a-zA-Z0-9\-]')),
            ],
            textCapitalization: TextCapitalization.characters,
            decoration: InputDecoration(
              hintText: 'Ex: ABC-123 ou UUID...',
              prefixIcon: const Icon(Icons.password_rounded),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: const BorderSide(
                  color: Color(0xFF1A3A8F),
                  width: 2,
                ),
              ),
            ),
            onSubmitted: (_) {
              if (_pinValido) {
                Navigator.of(context).pop(_controller.text.trim());
              }
            },
          ),
        ],
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text(
            'Cancelar',
            style: TextStyle(color: Color(0xFF64748B)),
          ),
        ),
        FilledButton(
          key: const Key('btn_confirmar_pin'),
          onPressed: _pinValido
              ? () => Navigator.of(context).pop(_controller.text.trim())
              : null,
          style: FilledButton.styleFrom(
            backgroundColor: const Color(0xFF1A3A8F),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(10),
            ),
          ),
          child: const Text('Confirmar'),
        ),
      ],
    );
  }
}
