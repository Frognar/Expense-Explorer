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

- **URL:** `/api/receipts?pageNumber=1&pageSize=10&sortBy=receivedDate&sortOrder=desc&search=stc&after=2021-10-02&before=2021-10-02&minTotal=1&maxTotal=10`
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
        "filteredCount": 1,
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
  ** `/api/incoems?pageNumber=1&pageSize=10&sortBy=receivedDate&sortOrder=desc&source=Wor&category=lary&description=onth&receivedAfter=2021-10-02&receivedBefore=2021-10-22&minAmount=1&maxAmount=2000`
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
        "filteredCount": 1,
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
        "filteredCount": 1,
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
        "filteredCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

### Categories

#### Get Categories

- **URL:** `/api/categories/expenses?pageNumber=1&pageSize=10&search=tre`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "categories": [
          "Treats"
        ],
        "totalCount": 1,
        "filteredCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```

### Sources

#### Get Sources

- **URL:** `/api/sources?pageNumber=1&pageSize=10&search=inv`
- **Method:** `GET`
- **Response Body:**
    ```json
    {
        "sources": [
          "Investments"
        ],
        "totalCount": 1,
        "filteredCount": 1,
        "pageSize": 10,
        "pageNumber": 1,
        "pageCount": 1
    }
    ```
