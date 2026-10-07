import 'package:flutter_test/flutter_test.dart';
import 'package:medica_mobile/main.dart';

void main() {
  testWidgets('Renders MedicaMobileApp smoke test', (WidgetTester tester) async {
    await tester.pumpWidget(const MedicaMobileApp(logado: false));
    expect(find.byType(MedicaMobileApp), findsOneWidget);
  });
}
