# Dokumentacja testów i jakości kodu

## 1. Zakres testów
- **BookingService (ReservationManager):**
  - Testy rezerwacji miejsca (dostępność, blokada użytkownika, nieistniejący użytkownik, nakładanie rezerwacji, graniczne czasy, edycja, anulowanie, status)
- **NotificationService:**
  - Testy powiadomień (do jednego i wielu odbiorców, puste dane, obsługa wyjątków)
- **PaymentProcessor:**
  - Testy przetwarzania płatności (poprawne, błędne, graniczne przypadki, mockowanie fabryki i płatności)
- **AuthService:**
  - Testy rejestracji (poprawna, istniejący email, słabe hasło), logowania (poprawne, złe dane, różne statusy), weryfikacji użytkownika

## 2. Mockowanie
- W testach użyto **Moq** do zamockowania zależności:
  - `IDatabaseService`, `NotificationService`, `IObserver` w testach rezerwacji
  - `INotificationFactory`, `IObserver` w testach powiadomień
  - `PaymentFactory`, `IPayment` w testach płatności
  - `IDatabaseService`, `NotificationService` w testach autoryzacji

## 3. Pokrycie kodu
- Pokrycie kodu mierzone narzędziem **dotnet-coverage** + **ReportGenerator**
- Raport HTML: `coverage-report/index.html`
- Kluczowe klasy (BookingService, PaymentProcessor, AuthService) mają wysokie pokrycie testami

## 4. Jak uruchomić testy i raport pokrycia
1. `dotnet test`
2. `dotnet-coverage collect 'dotnet test' -f cobertura -o coverage.xml`
3. `reportgenerator -reports:coverage.xml -targetdir:coverage-report -reporttypes:Html`
4. Otwórz `coverage-report/index.html` w przeglądarce

## 5. Podsumowanie
- Testy pokrywają kluczową logikę biznesową projektu
- Użyto mocków do izolacji testowanych klas
- Wszystkie testy przechodzą poprawnie (poza próbą mockowania nie-wirtualnych metod, co zostało poprawione)
- Pokrycie kodu wysokie
- Dokumentacja i raporty gotowe do oddania

---
**Wskazówki:**
- Testy powiadomień są osobno, nie należy mockować nie-wirtualnych metod NotificationService w innych testach
- W razie potrzeby można dodać kolejne testy dla innych modułów lub przypadków granicznych 