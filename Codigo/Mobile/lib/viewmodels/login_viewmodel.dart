import 'package:flutter/foundation.dart';
import '../services/auth_service.dart';

class LoginViewModel extends ChangeNotifier {
  bool _isLoading = false;
  String? _errorMessage;

  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<bool> autenticar(String tokenPareamento) async {
    if (_isLoading) return false;

    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    final resultado = await AuthService.associarDispositivo(
      tokenPareamento: tokenPareamento,
    );

    _isLoading = false;

    if (!resultado.sucesso) {
      _errorMessage = resultado.erro ?? 'Erro ao realizar autenticação.';
      notifyListeners();
      return false;
    }

    notifyListeners();
    return true;
  }

  void limparErro() {
    _errorMessage = null;
    notifyListeners();
  }
}
