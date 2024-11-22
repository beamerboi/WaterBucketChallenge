# Water Jug Challenge API 🚰

A .NET Web API that solves the classic Water Jug Riddle using the Breadth-First Search (BFS) algorithm.

## 🎯 The Challenge

You have two jugs:
- Jug X with capacity X liters
- Jug Y with capacity Y liters

Goal: Measure exactly Z liters of water using only these jugs.

### Allowed Operations
- Fill a jug completely
- Empty a jug completely
- Pour from one jug to another

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Any IDE (Visual Studio 2022 or VS Code recommended)
- [Docker](https://www.docker.com/get-started)

### Run Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/beamerboi/WaterBucketChallenge.git
   ```
2. Navigate to the project directory:
   ```bash
   cd WaterBucketChallenge
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

### Run with Docker
1. Build the Docker image:
   ```bash
   docker build -t water-jug-challenge-api .
   ```
2. Run the Docker container:
   ```bash
   docker run -p 32768:32768 water-jug-challenge-api
   ```

### Run With Visual Studio 2022
1. Open the solution using VS 2022.
2. Start the service using IIS.

## 📡 API Endpoints

### POST /WaterJug/solve
- **URI**: `/WaterJug/solve`
- **Method**: POST
- **Description**: Solves the Water Jug Riddle for given jug capacities and target volume.
- **Request Parameters**:
  - `jugXCapacity` (int): Capacity of Jug X.
  - `jugYCapacity` (int): Capacity of Jug Y.
  - `targetVolume` (int): Desired volume to measure.

- **Request Example**:
  ```http
  POST /WaterJug/solve
  ```
- **Example of JSON request body**:
```json
  {
      "XCapacity": 3,
      "YCapacity": 5,
      "ZAmountWanted": 4
  }
```

- **Response Format**:
  ```json
  {
    "solution": [
        {
            "step": 1,
            "bucketX": 0,
            "bucketY": 5,
            "action": "Fill Bucket Y",
            "status": null
        },
        {
            "step": 2,
            "bucketX": 3,
            "bucketY": 2,
            "action": "Transfer Y to X",
            "status": null
        },
        {
            "step": 3,
            "bucketX": 0,
            "bucketY": 2,
            "action": "Empty Bucket X",
            "status": null
        },
        {
            "step": 4,
            "bucketX": 2,
            "bucketY": 0,
            "action": "Transfer Y to X",
            "status": null
        },
        {
            "step": 5,
            "bucketX": 2,
            "bucketY": 5,
            "action": "Fill Bucket Y",
            "status": null
        },
        {
            "step": 6,
            "bucketX": 3,
            "bucketY": 4,
            "action": "Transfer Y to X",
            "status": "Solved"
        }
    ]
  }
  ```
- **Status Codes**:
  - `200 OK`: Solution found and returned.
  - `400 Bad Request`: Invalid input parameters.
  - `500 Internal Server Error`: An error occurred while processing the request.

## 🧠 Algorithm Explanation

The Water Jug Challenge is solved using the Breadth-First Search (BFS) algorithm. BFS is used to explore all possible states of the jugs systematically. The algorithm works as follows:
1. Start with both jugs empty.
2. Use a queue to explore each possible state of the jugs.
3. For each state, perform all possible operations (fill, empty, pour) to generate new states.
4. Check if the target volume is reached in either jug.
5. If the target is reached, return the sequence of operations.
6. If all states are explored without reaching the target, return failure.

## 🛠️ Development

### Testing
- Run unit tests:
  ```bash
  dotnet test
  ```

*The project was created for Chicks Gold interview*