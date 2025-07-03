# System Parkingowy – Dokumentacja techniczna i projektowa

---

## Spis treści
1. [Opis systemu i użytkowników](#opis-systemu-i-użytkowników)
2. [Ograniczenia i wymagania niefunkcjonalne](#ograniczenia-i-wymagania-niefunkcjonalne)
3. [Analiza danych i środowiska](#analiza-danych-i-środowiska)
4. [Wymagania funkcjonalne i niefunkcjonalne](#wymagania-funkcjonalne-i-niefunkcjonalne)
5. [Opis działania systemu (na podstawie Program.cs)](#opis-działania-systemu-na-podstawie-programcs)
6. [Diagram przypadków użycia UML](#diagram-przypadków-użycia-uml)
7. [Architektura systemu – diagram komponentów UML](#architektura-systemu--diagram-komponentów-uml)
8. [Diagram klas UML (przykład)](#diagram-klas-uml-przykład)
9. [Diagramy aktywności UML](#diagramy-aktywności-uml)
10. [Wzorce projektowe – analiza i dobór](#wzorce-projektowe--analiza-i-dobór)
11. [Diagramy klas wzorców projektowych](#diagramy-klas-wzorców-projektowych)
12. [Implementacja wzorców w systemie](#implementacja-wzorców-w-systemie)
13. [Typy kodu, technologie, narzędzia](#typy-kodu-technologie-narzędzia)

---

## Struktura projektu – dodatkowe części

### System-Parkingowy-Lib-

- Zawiera logikę biznesową systemu parkingowego w formie biblioteki.
- Udostępnia klasy, interfejsy i implementacje modułów: rezerwacji, płatności, powiadomień, autoryzacji, bazy danych.
- W katalogu `Tests/SystemParkingowy.Tests/` znajdują się testy jednostkowe i integracyjne dla kluczowych komponentów:
  - `AuthServiceTests.cs` – testy autoryzacji i rejestracji użytkowników.
  - `BookingServiceTests.cs` – testy procesu rezerwacji.
  - `NotificationServiceTests.cs` – testy powiadomień.
  - `PaymentProcessorTests.cs` – testy obsługi płatności.
- Biblioteka jest wykorzystywana przez aplikację konsolową oraz przez API.

#### Uruchamianie testów

```sh
cd System-Parkingowy-Lib-/Tests/SystemParkingowy.Tests
# .NET CLI:
dotnet test
```

![image](https://github.com/user-attachments/assets/5653c85e-7703-4f0d-806f-47b09f3327d4)


#### Pokrycie testów (coverage)

Po uruchomieniu testów generowany jest raport pokrycia w pliku `coverage.xml` oraz w katalogu `coverage-report/`.

**Miejsce na zrzut ekranu z raportu pokrycia:**

![image](https://github.com/user-attachments/assets/246bcf14-17e5-4ade-a7d1-60ab939ea829)

### System-Parkingowy-API

- Udostępnia funkcjonalność systemu parkingowego przez REST API.
- Implementuje kontrolery:
  - `ParkingSpotController` – zarządzanie miejscami parkingowymi.
  - `ReservationController` – obsługa rezerwacji.
  - `AuthController` – rejestracja, logowanie, autoryzacja.
  - `UserController` – zarządzanie kontami użytkowników.
  - `PaymentController` – obsługa płatności.
  - `PredictionController` – przewidywanie dostępności miejsc.
  - `SensorController` – integracja z czujnikami parkingowymi.
  - `SimulationController` – symulacje działania systemu.
- API korzysta z logiki biznesowej z biblioteki System-Parkingowy-Lib-.

#### Dokumentacja API (Swagger)

Po uruchomieniu projektu API dostępna jest dokumentacja Swagger pod adresem: http://localhost:5000/swagger/index.html

![image](https://github.com/user-attachments/assets/7924b8bf-0502-443a-9eca-7be6d657e1e7)


### System-Parkingowy-Console-

- Aplikacja konsolowa korzystająca z biblioteki.

![image](https://github.com/user-attachments/assets/9ecf3154-70b7-4483-bf38-27f2fc287637)


## Opis systemu i użytkowników

System parkingowy służy do zarządzania rezerwacjami miejsc parkingowych w firmie lub instytucji. Pozwala użytkownikom na rejestrację, logowanie, przeglądanie dostępnych miejsc, dokonywanie rezerwacji, płatności oraz otrzymywanie powiadomień.

**Aktorzy systemu:**
- **Użytkownik (Pracownik/Gość):**
  - Rejestruje konto
  - Loguje się do systemu
  - Przegląda dostępność miejsc parkingowych
  - Rezerwuje miejsce parkingowe
  - Anuluje rezerwację
  - Otrzymuje powiadomienia (email, SMS, push)
  - Dokonuje płatności za rezerwację (BLIK, karta, PayPal)
- **Administrator:**
  - Zarządza użytkownikami
  - Zarządza miejscami parkingowymi
  - Przegląda statystyki i raporty
  - Konfiguruje powiadomienia i płatności

---

## Ograniczenia i wymagania niefunkcjonalne

- System musi być zgodny z RODO (ochrona danych osobowych)
- Dostępność 24/7, wysoka niezawodność
- Bezpieczeństwo: szyfrowanie danych, autoryzacja, logowanie zdarzeń
- Wydajność: obsługa min. 100 jednoczesnych użytkowników
- Skalowalność: możliwość rozbudowy o kolejne parkingi/lokalizacje
- Integracja z zewnętrznymi systemami płatności i powiadomień
- Możliwość wdrożenia w chmurze lub lokalnie
- Wsparcie dla urządzeń mobilnych i desktopowych

---

## Analiza danych i środowiska

- Liczba użytkowników: do 200 (pracownicy, goście, administratorzy)
- Przetwarzane dane: dane osobowe, rezerwacje, płatności, powiadomienia
- Lokalizacja: siedziba firmy, możliwość pracy zdalnej (VPN)
- Sprzęt: serwer Windows/Linux, komputery pracowników, smartfony
- Oprogramowanie: .NET Core, MS SQL/PostgreSQL, SMTP, API płatności
- Możliwość integracji z systemami kontroli dostępu (szlabany, RFID)

---

## Wymagania funkcjonalne i niefunkcjonalne

### Wymagania funkcjonalne

1. Rejestracja i logowanie użytkowników
2. Przeglądanie dostępnych miejsc parkingowych
3. Rezerwacja i anulowanie rezerwacji
4. Obsługa płatności online (BLIK, karta, PayPal)
5. Wysyłka powiadomień (email, SMS, push)
6. Zarządzanie użytkownikami i miejscami (admin)
7. Generowanie raportów i statystyk
8. Integracja z systemami zewnętrznymi (API)

### Wymagania niefunkcjonalne

1. Bezpieczeństwo danych (RODO, szyfrowanie, autoryzacja)
2. Wydajność i skalowalność
3. Niezawodność i dostępność 24/7
4. Przenośność (chmura/lokalnie)
5. Łatwość obsługi (UI/UX)
6. Zgodność z przepisami prawa
7. Możliwość rozbudowy i integracji

---

## Opis działania systemu (na przykładzie Program.cs)

![image](https://github.com/user-attachments/assets/55631423-f77c-4c04-b684-b4d78f2f3d04)


System uruchamia się poprzez klasę `Program` i korzysta z fasady `ParkingSystemFacade`, która upraszcza interakcję z głównymi modułami systemu:

1. **Rejestracja i weryfikacja użytkownika:**
   - Użytkownik podaje email i hasło.
   - System rejestruje użytkownika i automatycznie weryfikuje konto.
   - Dane użytkownika są zapisywane w bazie danych.
2. **Rezerwacja miejsca parkingowego:**
   - Użytkownik wybiera miejsce, datę i godzinę.
   - System sprawdza dostępność miejsca i tworzy rezerwację.
   - Rezerwacja jest zapisywana w bazie danych.
   - Użytkownik otrzymuje powiadomienie o rezerwacji.
3. **Zmiana strategii opłat:**
   - Administrator lub system może ustawić inną strategię naliczania opłat (np. dla VIP).
4. **Płatność za rezerwację:**
   - Użytkownik wybiera metodę płatności (BLIK, karta, PayPal).
   - System przetwarza płatność przez odpowiedni moduł.
   - Po opłaceniu rezerwacji użytkownik otrzymuje powiadomienie.
5. **Anulowanie rezerwacji:**
   - Użytkownik może anulować rezerwację, a system aktualizuje status i powiadamia użytkownika.

**Przykładowy scenariusz (z Program.cs):**
- Użytkownik rejestruje się i weryfikuje konto.
- Rezerwuje miejsce parkingowe na wybrany termin.
- Zmienia strategię opłat na VIP.
- Rezerwuje kolejne miejsce na inny termin.

- ![image](https://github.com/user-attachments/assets/4222e857-cf37-4d67-a76d-a0f9b5f58273)

---

## Diagram klas UML (przykład – Moduł Rezerwacji)

```mermaid
classDiagram
    class ReservationManager {
        +SearchParkingSpot(location)
        +MakeReservation(reservation)
        +EditReservation(id, newStart, newEnd)
        +CancelReservation(id)
        +PayForReservation(id, factory)
        +SetFeeStrategy(strategy)
    }
    class Reservation {
        +Id: int
        +UserId: int
        +ParkingSpot: ParkingSpot
        +StartTime: DateTime
        +EndTime: DateTime
        +Status: ReservationStatus
        +TotalPrice: decimal
        +Confirm()
        +Cancel()
    }
    class ParkingSpot {
        +Id: int
        +Location: string
        +Zone: string
        +Available: bool
        +MarkFree()
        +MarkOccupied()
    }
    ReservationManager --> Reservation : zarządza
    ReservationManager --> ParkingSpot : sprawdza dostępność
    Reservation o-- ParkingSpot : dotyczy
    Reservation o-- User : należy do
    class User {
        +Id: int
        +Email: string
        +PhoneNumber: string
        +Password: string
        +Status: UserStatus
        +Activate()
        +Block()
        +Unblock()
        +Delete()
    }
```

---

## Diagramy aktywności UML

### a) Scenariusz użytkownika (UI-perspective) – Rezerwacja miejsca

```mermaid
flowchart TD
    A([Start]) --> B[Logowanie do systemu]
    B --> C[Wyświetlenie listy miejsc]
    C --> D[Wybór miejsca i daty]
    D --> E{Czy miejsce dostępne?}
    E -- Tak --> F[Potwierdzenie rezerwacji]
    E -- Nie --> C
    F --> G[Otrzymanie powiadomienia]
    G --> H([Koniec])
```

### b) Logika systemu (System-perspective) – Rezerwacja miejsca

```mermaid
flowchart TD
    A([Start]) --> B[Sprawdzenie autoryzacji]
    B --> C[Walidacja danych rezerwacji]
    C --> D{Miejsce dostępne?}
    D -- Tak --> E[Zapis rezerwacji do bazy]
    D -- Nie --> F[Zgłoszenie błędu]
    E --> G[Wysłanie powiadomienia]
    G --> H([Koniec])
    F --> H
```

### c) Proces techniczny (Backend/serwis) – Rezerwacja i płatność

```mermaid
flowchart TD
    A([Start]) --> B[Odbiór żądania rezerwacji]
    B --> C[Sprawdzenie dostępności]
    C --> D{Dostępne?}
    D -- Tak --> E[Rezerwacja w bazie]
    E --> F[Inicjacja płatności]
    F --> G{Płatność OK?}
    G -- Tak --> H[Wysłanie powiadomienia]
    G -- Nie --> I[Anulowanie rezerwacji]
    H --> J([Koniec])
    I --> J
    D -- Nie --> J
```

---

## Wzorce projektowe – analiza i dobór

W projekcie zastosowano następujące wzorce projektowe:
- **Factory Method:** Tworzenie różnych typów powiadomień (Email, SMS, Push)
- **Abstract Factory:** Tworzenie rodzin powiązanych obiektów powiadomień (Standard/Enterprise)
- **Adapter:** Integracja z zewnętrznymi systemami płatności (np. PayU)
- **Observer:** Powiadamianie wielu odbiorców o zmianach rezerwacji
- **Strategy:** Różne strategie naliczania opłat (np. Standard, VIP)
- **Facade:** Uproszczenie interfejsu do głównych funkcji systemu (ParkingSystemFacade)

---

## Diagramy klas wzorców projektowych

### Factory Method (Notifier)

```mermaid
classDiagram
    class Notifier {
        <<interface>>
        +Send(message)
    }
    class EmailNotifier {
        +Send(message)
    }
    class SmsNotifier {
        +Send(message)
    }
    Notifier <|.. EmailNotifier
    Notifier <|.. SmsNotifier
    class NotificationFactory {
        +CreateNotifier(type): Notifier
    }
    NotificationFactory --> Notifier
```

### Abstract Factory (NotificationFactory)

```mermaid
classDiagram
    class INotificationFactory {
        <<interface>>
        +CreateEmailNotifier()
        +CreateSmsNotifier()
        +CreatePushNotifier()
    }
    class StandardNotificationFactory {
        +CreateEmailNotifier()
        +CreateSmsNotifier()
        +CreatePushNotifier()
    }
    class EnterpriseNotificationFactory {
        +CreateEmailNotifier()
        +CreateSmsNotifier()
        +CreatePushNotifier()
    }
    INotificationFactory <|.. StandardNotificationFactory
    INotificationFactory <|.. EnterpriseNotificationFactory
```

### Adapter (PayUAdapter)

```mermaid
classDiagram
    class IPayment {
        <<interface>>
        +Pay(amount)
    }
    class PayUAdapter {
        +Pay(amount)
    }
    IPayment <|.. PayUAdapter
```

### Observer (NotificationService)

```mermaid
classDiagram
    class IObserver {
        <<interface>>
        +Update()
    }
    class ISubject {
        <<interface>>
        +Attach(observer)
        +Detach(observer)
        +Notify()
    }
    class NotificationService {
        +Attach(observer)
        +Detach(observer)
        +Notify()
    }
    NotificationService ..|> ISubject
    EmailNotifier ..|> IObserver
    SmsNotifier ..|> IObserver
    NotificationService --> IObserver
```

### Strategy (FeeStrategy)

```mermaid
classDiagram
    class IFeeStrategy {
        <<interface>>
        +CalculateFee(reservation)
    }
    class StandardFeeStrategy {
        +CalculateFee(reservation)
    }
    class VipFeeStrategy {
        +CalculateFee(reservation)
    }
    IFeeStrategy <|.. StandardFeeStrategy
    IFeeStrategy <|.. VipFeeStrategy
```

### Facade (ParkingSystemFacade)

```mermaid
classDiagram
    class ParkingSystemFacade {
        +RegisterAndVerifyUser(email, password)
        +BookSpot(user, spotId, start, end)
        +PayForReservation(reservationId, factory)
        +CancelReservation(reservationId)
        +SetFeeStrategy(strategy)
    }
    ParkingSystemFacade --> ReservationManager
    ParkingSystemFacade --> AuthService
    ParkingSystemFacade --> DatabaseService
    ParkingSystemFacade --> NotificationService
```

---

## Typy kodu, technologie, narzędzia

- **Język:** C# (.NET Core)
- **Baza danych:** MS SQL Server / PostgreSQL
- **Wzorce projektowe:** Factory Method, Abstract Factory, Adapter, Observer, Strategy, Facade
- **Diagramy:** UML (Mermaid, PlantUML)
- **Powiadomienia:** Email (SMTP), SMS, Push
- **Płatności:** BLIK, Karta, PayPal, PayU (Adapter)
- **Testy:** xUnit/NUnit (propozycja)
- **Narzędzia:** Visual Studio, Git, Mermaid Live Editor

---
