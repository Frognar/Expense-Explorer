<p align="center">
    <img alt="Expense-Explorer-Logo" src="res/imgs/Expense-Explorer-Logo-512.png">
</p>

## Expense-Explorer

[![100 - commitów](https://img.shields.io/badge/100-commitów-2ea44f?logo=csharp)](https://100commitow.pl/)
[![.net workflow](https://github.com/Frognar/Expense-Explorer/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Frognar/Expense-Explorer/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Expense Explorer is a simple expense tracking application designed to run on a home server, allowing users to manage their receipts and track expenses conveniently. This project is written in C# and serves as a learning opportunity, particularly exploring concepts like Event-sourcing.

## Features

- [x] **Receipt Management:** Add, edit, and delete receipts.
- [x] **Purchase Management:** Add, edit, and delete purchases associated with receipts.
- [X] **Browse Receipts:** View and search through receipts based on store, date, and total amount.
- [x] **Browse Stores, Items, and Categories:** View and search through stores, items, and categories used in receipts
  and purchases.
- [x] **Reporting:** Generate category-wise expense reports for a given date range.

## Nice to Have

- [ ] **GUI:** A simple web-based GUI to interact with the application.
- [ ] **OCR Integration:** Automatically extract receipt details using OCR.
- [x] **Income Tracking:** Track income and compare with expenses.

## Note

- This application is intended for personal use and is not designed for multi-user environments. Receipts are not associated with specific users.
- The primary purpose of this project is to explore and learn new technologies, specifically Event Sourcing in this case.
- Future development may include additional features.

I know that instead of implementing certain functionalities myself, I should use ready-made solutions (for example, MediatR for handling requests), but I also wanted to use this project to expand my knowledge of how such libraries work. It is possible that in the future, I will replace my own implementations with better, more tested packages.

## Getting Started

The project is currently under development. Stay tuned for future updates on setup and usage instructions.
API is already implemented and can be used, [documentation](#API) is available below.

1. **Clone the Repository:** Begin by cloning the Expense Explorer repository to your local machine:
    ```bash
    git clone https://github.com/Frognar/Expense-Explorer.git
    ```
2. **Set Up Environment:** Ensure you have the necessary environment set up to run the project.
    - [.NET SDK ](https://dotnet.microsoft.com/download)
    - [Docker Desktop](https://www.docker.com/products/docker-desktop/).

3. **Build and Run:** Navigate to the solution directory and run *build.sh* script to build the project:
    ```bash
    ./build.sh
    ```

   Once the build process is complete, you can execute *run.sh* script to start the application:
    ```bash
    ./run.sh
    ```

   The application should now be running and accessible at `http://localhost:5000`.
4. **Explore API Endpoints:** With the application running, you can explore the API endpoints provided. For example, you
   can interact with the receipt endpoints by sending HTTP requests to `/api/receipts`. You can use tools like Postman
   or curl to make requests.

## API

### Receipts

#### Add Receipt

- **URL:** `/api/receipts`
- **Method:** `POST`
- **Request Body:**
    ```json
    {
        "storeName": "Walmart",
        "purchaseDate": "2021-10-01"
    }
    ```
- **Response Body:**
    ```json
    {
        "id": "386c7c4014304b039d23f99fe9d9869e",
        "storeName": "Walmart",
        "purchaseDate": "2021-10-01",
        "version": 0
    }
    ```

#### Edit Receipt

- **URL:** `/api/receipts/{id}`
- **Method:** `PATCH`
- **Request Body:**
    ```json
    {
        "storeName": "Costco",        // optional
        "purchaseDate": "2021-10-02"  // optional
    }
    ```
- **Response Body:**
    ```json
    {
        "id": "386c7c4014304b039d23f99fe9d9869e",
        "storeName": "Costco",
        "purchaseDate": "2021-10-02",
        "version": 2
    }
    ```

#### Delete Receipt

- **URL:** `/api/receipts/{id}`
- **Method:** `DELETE`

#### Add Purchase

- **URL:** `/api/receipts/{id}/purchases`
- **Method:** `POST`
- **Request Body:**
    ```json
    {
      "item": "Milk",
      "Category": "Groceries",
      "quantity": 2,
      "unitPrice": 2.5,
      "totalDiscount": 0.5,
      "description": "2% milk"  // optional
    }
    ```
- **Response Body:**
    ```json
    {
        "id": "386c7c4014304b039d23f99fe9d9869e",
        "storeName": "Costco",
        "purchaseDate": "2021-10-02",
        "purchases": [
            {
                "id": "bd0e0d05518d486dbbfce32df7e6da3b",
                "item": "Milk",
                "category": "Groceries",
                "quantity": 2,
                "unitPrice": 2.5,
                "totalDiscount": 0.5,
                "description": "2% milk"
            }
        ],
        "version": 3
    }
    ```

#### Edit Purchase

- **URL:** `/api/receipts/{id}/purchases/{purchaseId}`
- **Method:** `PATCH`
- **Request Body:**
    ```json
    {
        "item": "Coca Cola",    // optional
        "Category": "Treats",   // optional
        "quantity": 1,          // optional
        "unitPrice": 1.5,       // optional
        "totalDiscount": 0,     // optional
        "description": "Coke"   // optional
    }
    ```
- **Response Body:**
    ```json
    {
        "id": "386c7c4014304b039d23f99fe9d9869e",
        "storeName": "Costco",
        "purchaseDate": "2021-10-02",
        "purchases": [
            {
                "id": "bd0e0d05518d486dbbfce32df7e6da3b",
                "item": "Coca Cola",
                "category": "Treats",
                "quantity": 1,
                "unitPrice": 1.5,
                "totalDiscount": 0,
                "description": "Coke"
            }
        ],
        "version": 4
    }
    ```

#### Delete Purchase

- **URL:** `/api/receipts/{id}/purchases/{purchaseId}`
- **Method:** `DELETE`

#### Get Receipts

- **URL:** `/api/receipts?pageNumber=1&pageSize=10&search=stc&after=2021-10-02&before=2021-10-02&minTotal=1&maxTotal=10`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "receipts": [
            {
                "id": "386c7c4014304b039d23f99fe9d9869e",
                "store": "Costco",
                "purchaseDate": "2021-10-02",
                "total": 1.5
            }
        ],
        "totalCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

#### Get Receipt

- **URL:** `/api/receipts/{id}`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "id": "386c7c4014304b039d23f99fe9d9869e",
        "store": "Costco",
        "purchaseDate": "2021-10-02",
        "total": 1.5,
        "purchases": [
            {
                "id": "bd0e0d05518d486dbbfce32df7e6da3b",
                "item": "Coca Cola",
                "category": "Treats",
                "quantity": 1,
                "unitPrice": 1.5,
                "totalDiscount": 0,
                "description": "Coke"
            }
        ]
    }
    ```

### Income

#### Add Income

- **URL:** `/api/incomes`
- **Method:** `POST`
- **Request Body:**
    ```json
    {
    "source": "Work",
    "amount": 1500,
    "category": "Salary",
    "receivedDate": "2021-10-10",
    "description": "Monthly salary"
    }
    ```
- **Response Body:**
    ```json
    {
      "id": "da5da2ff8d7545f1b7f2b7da7647ea51",
      "source": "Work",
      "amount": 1500,
      "category": "Salary",
      "receivedDate": "2021-10-10",
      "description": "Monthly salary",
      "version": 0
    }
    ```

#### Edit Income

- **URL:** `/api/incomes/{incomeId}`
- **Method:** `PATCH`
- **Request Body:**
    ```json
    {
        "source": "Investments",
        "amount": 200,
        "category": "Dividends",
        "receivedDate": "2021-10-12",
        "description": "Quarterly dividends"
    }
    ```
- **Response Body:**
    ```json
    {
      "id": "da5da2ff8d7545f1b7f2b7da7647ea51",
      "source": "Investments",
      "amount": 200,
      "category": "Dividends",
      "receivedDate": "2021-10-12",
      "description": "Quarterly dividends",
      "version": 5
    }
    ```

#### Delete Income

- **URL:** `/api/incomes/{incomeId}`
- **Method:** `DELETE`

#### Get Incomes

- **URL:
  ** `/api/incoems?pageNumber=1&pageSize=10&source=Wor&category=lary&description=onth&receivedAfter=2021-10-02&receivedBefore=2021-10-22&minAmount=1&maxAmount=2000`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "incomes": [
            {
                "id": "da5da2ff8d7545f1b7f2b7da7647ea51",
                "source": "Investments",
                "amount": 200,
                "category": "Dividends",
                "receivedDate": "2021-10-12",
                "description": "Quarterly dividends"
            }
        ],
        "totalCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

#### Get Income

- **URL:** `/api/incomes/{id}`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "id": "da5da2ff8d7545f1b7f2b7da7647ea51",
        "source": "Investments",
        "amount": 200,
        "category": "Dividends",
        "receivedDate": "2021-10-12",
        "description": "Quarterly dividends"
    }
    ```

### Reports

#### Get Report

- **URL:** `/api/reports/category-based-expense?from=2021-10-01&to=2021-10-31`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "startDate": "2021-10-01",
        "endDate": "2021-10-31",
        "total": 1.5,
        "categories": [
            {
                "category": "Treats",
                "total": 1.5
            }
        ]
    }
    ```

- **URL:** `/api/reports/income-to-expense?from=2021-10-01&to=2021-10-31`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "startDate": "2021-10-01",
        "endDate": "2021-10-31",
        "totalIncome": 200,
        "totalExpense": 1.5
    }
    ```

### Stores

#### Get Stores

- **URL:** `/api/stores?pageNumber=1&pageSize=10&search=cos`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "stores": [
          "Costco"
        ],
        "totalCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

### Items

#### Get Items

- **URL:** `/api/items?pageNumber=1&pageSize=10&search=cola`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "items": [
          "Coca Cola"
        ],
        "totalCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

### Categories

#### Get Categories

- **URL:** `/api/categories?pageNumber=1&pageSize=10&search=tre`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "categories": [
          "Treats"
        ],
        "totalCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

## Contributing

Contributions to Expense Explorer are welcome! Whether it's bug fixes, new features, or enhancements, feel free to submit pull requests.

## License

This project is licensed under the [MIT License](LICENSE). Feel free to use, modify, and distribute as per the terms of the license.
