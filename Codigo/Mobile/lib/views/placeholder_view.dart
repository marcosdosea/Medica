import 'package:flutter/material.dart';
import '../config/app_colors.dart';
import '../config/session_manager.dart';
import 'login_view.dart';

class PlaceholderView extends StatelessWidget {
  const PlaceholderView({super.key});

  Future<void> _logout(BuildContext context) async {
    await SessionManager.clear();
    if (!context.mounted) return;
    Navigator.of(context).pushReplacement(
      MaterialPageRoute(builder: (_) => const LoginView()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: AppColors.primaryMedium,
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.logout_rounded),
            tooltip: 'Sair',
            onPressed: () => _logout(context),
          ),
        ],
      ),
      body: const Padding(
        padding: EdgeInsets.all(16.0),
        child: Placeholder(),
      ),
    );
  }
}
