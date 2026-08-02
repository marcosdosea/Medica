import 'package:flutter_test/flutter_test.dart';
import 'package:medica_mobile/main.dart';

void main() {
  testWidgets('Renders MedicaMobileApp home page smoke test', (WidgetTester tester) async {
    await tester.pumpWidget(const MedicaMobileApp());
    expect(find.text('Medica Mobile'), findsOneWidget);
  });
}
