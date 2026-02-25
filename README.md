# Cluster Matrix

Console application written in C# (.NET 8) that generates square matrices filled with random numbers and analyzes clusters of adjacent equal values.

## Task

The program:

- Generates square matrices populated with random numbers in range **0–12**
- Detects clusters of **adjacent elements** (up, down, left, right — no diagonals)
- Counts only clusters with **size greater than 5**
- Runs the simulation across **5513 matrices**
- Calculates:
    - Average number of qualifying clusters per matrix
    - Average cluster size
    - Most frequently occurring digit(s)

Matrix size is provided via a **command-line argument**.

---

## How to run

Open a terminal in the project directory and run:

```sh
dotnet run -- 50
```
