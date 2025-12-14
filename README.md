# 💵 Payment Gateway

This API, developed using .NET, functions as a simulated Payment Gateway. It allows merchants to process card payments for their shoppers and includes an integrated simulation of an Acquiring Bank for handling payment authorization.

## 📈 Payment Outcome Statuses

Payment processing can result in one of three distinct statuses, which dictates the HTTP response returned to the merchant:

| Status | Origin | Description | HTTP Status Code |
| :--- | :--- | :--- | :--- |
| **Rejected** | Internal (Payment Gateway) | **No payment was created.** Invalid or malformed information was supplied in the request, failing early validation. | `400 Bad Request` |
| **Authorized** | External (Acquiring Bank) | The payment was successfully approved by the acquiring bank simulator. | `200 OK` |
| **Declined** | External (Acquiring Bank) | The payment was rejected by the acquiring bank simulator. | `200 OK` |

## 🏗️ Design and Architecture Considerations

### Layered Architecture

The application follows a **layered architecture** **Separation of Concerns** and **Testability**. Although the design adheres to a layered structure, for the purpose of this simple technical assessment/simulation, the entire application is organized within a single .NET project (PaymentGateway.Api).

* **Controller Layer:** Handles HTTP request/response mapping and initial request validation.
* **Use Case Layer:** Contains the core business logic (e.g., orchestrating the bank call, handling the result, and persisting the data).
* **Repository Layer:** Abstracted data storage (implemented with an in-memory repository).

### Explicit Error Handling (The Result Pattern)

The design uses the **Result Pattern** extensively, facilitated by `FluentResults`, to clearly separate business success from business failure:

* **The Controller checks the result of the validation:**
    * If validation fails, it returns `400 Bad Request` with validation errors (`Rejected` status).
* **The Use Case manages the bank interaction:**
    * If the bank call succeeds, the result is packaged as `Success` (containing the `Authorized` or `Declined` payment object).
    * If the bank client encounters a severe error (like a transient fault), it returns a `Failure` Result, which the controller handles by returning a relevant error code.

## 💻 Tech Stack & Dependencies
* **.NET 8**
* Packages Used:
    * **RestEase** - Used to create a simple, type-safe client interface for communicating with the external Acquiring Bank Simulator API. This abstracts away the direct HTTP client implementation.
    * **FluentResults** - Implements the strict validation rules for incoming payment requests (e.g., card length, future expiry date, CVV format). This allows for clean, separate, and easy-to-read validation logic, resulting in Rejected payments when rules fail.
    * **FluentValidation** - Provides a robust, non-exception-based mechanism for handling the distinct outcomes of business operations, clearly separating success (Authorized / Declined) from failure (validation errors).
    * **Polly** - Implements resilience strategies, such as retries, for handling transient network errors or timeouts when calling the external Acquiring Bank Simulator. This ensures the Payment Gateway is reliable.

* Testing:
    * **xUnit** - The chosen testing framework used to define and execute tests. It provides the core structures for writing test methods and assertions.
    * **Moq** - A mocking framework used to create mock objects for dependencies (like the in-memory repository or the RestEase bank client)
    * **AutoBogus** - A library used for generating realistic, automated fake data

## 