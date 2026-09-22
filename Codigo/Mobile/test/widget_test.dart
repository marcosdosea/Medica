import 'package:flutter_test/flutter_test.dart';
import 'package:medica_mobile/main.dart';

void main() {
  testWidgets('Renders MedicaMobileApp home page smoke test', (WidgetTester tester) async {
    // logado: false -> exibe LoginView (não depende de sessão real)
    await tester.pumpWidget(const MedicaMobileApp(logado: false));
    expect(find.byType(MedicaMobileApp), findsOneWidget);
  });
}
