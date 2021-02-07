using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix : MonoBehaviour
{
    public int columnCount;
    public int rowCount;

    public float[,] matrix;

    // Konstruktor der Matrix aus zwei Werten
    public Matrix(int _columnCount, int _rowCount)
    {
        columnCount = _columnCount;
        rowCount = _rowCount;

        matrix = new float[columnCount, rowCount];
    }

    // Konstruktor der Matrix aus 2D Array
    public Matrix(float[,] _array)
    {
        columnCount = _array.GetLength(0);
        rowCount = _array.GetLength(1);

        matrix = new float[columnCount, rowCount];
    }

    // Matrix Multiplikation
    public static Matrix operator *(Matrix _a, Matrix _b)
    {
        return new Matrix(0, 0);
    }

    // Matrix Multiplikation mit Skalar
    public static Matrix operator *(Matrix _a, float _b)
    {
        return new Matrix(0, 0);
    }

    // Matrix Addition
    public static Matrix operator +(Matrix _a, Matrix _b)
    {
        return new Matrix(0, 0);
    }

    // Matrix Addition mit Skalar
    public static Matrix operator +(Matrix _a, float _b)
    {
        return new Matrix(0, 0);
    }

    // Nimmt zwei Matrizen multipliziert diese und gibt eine neue Matrix zurück
    public Matrix MultiplyMatrices(Matrix _matrixB)
    {
        Matrix _newMatrix = new Matrix(columnCount, _matrixB.rowCount);
        float _temp;

        if (rowCount == _matrixB.columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 0; j < _matrixB.rowCount; j++)
                {
                    _temp = 0;

                    for (int k = 0; k < rowCount; k++)
                    {
                        _temp += matrix[i, k] * _matrixB.matrix[k, j];
                    }

                    _newMatrix.matrix[i, j] = _temp;
                }
            }

            return _newMatrix;
        }
        else
        {
            if (rowCount == _matrixB.rowCount)
            {
                Debug.Log("Transpose");

                _matrixB.TransposeMatrix();

                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 0; j < _matrixB.rowCount; j++)
                    {
                        _temp = 0;

                        for (int k = 0; k < rowCount; k++)
                        {
                            _temp += matrix[i, k] * _matrixB.matrix[k, j];
                        }

                        _newMatrix.matrix[i, j] = _temp;
                    }
                }

                return _newMatrix;
            }
            else
            {
                Debug.Log("Matrices incompatible!");
            }
        }

        return null;
    }

    // Transponiert die gesamte Matrix
    public Matrix TransposeMatrix()
    {
        Matrix _newMatrix = new Matrix(rowCount, columnCount);

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                _newMatrix.matrix[i, j] = matrix[j, i];
            }
        }

        return _newMatrix;
    }

    // Multipliziert die gesamte Matrix mit einem Skalar
    public void MultiplyWithScalar(float _scalar)
    {
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                matrix[i, j] *= _scalar;
            }
        }
    }

    // Addiert einen Skalar zur Matrix
    public Matrix AddScalar(float _scalar)
    {
        Matrix _matrix = new Matrix(columnCount, rowCount);

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                matrix[i, j] += _scalar;
            }
        }

        _matrix.matrix = matrix;

        return _matrix;
    }

    // Wandelt die Matrix zu einem 2D Array um
    public float[,] ToArray()
    {
        float[,] _newArray = new float[columnCount, rowCount];

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                _newArray[i, j] = matrix[i, j];
            }
        }

        return _newArray;
    }

    // Weisst jedem Wert der Matrix den Wert 0 zu
    public void Clear()
    {
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                matrix[i, j] = 0;
            }
        }
    }

    // Weisst jedem Wert der Matrix eine zufällige Zahl zwischen -1 und 1 zu
    public void RandomValue()
    {
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                matrix[i, j] = Random.Range(-1f, 1f);
            }
        }
    }

    // Aktiviert die Matrix mit der Sigmoid Funktion
    public Matrix ActivateSigmoid()
    {
        Matrix _newMatrix = new Matrix(columnCount, rowCount);

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                _newMatrix.matrix[i, j] = Sigmoid(matrix[i, j]);
            }
        }

        return _newMatrix;
    }

    // Aktiviert die Matrix mit der Tanh Funktion
    public Matrix ActivateTanh()
    {
        Matrix _newMatrix = new Matrix(columnCount, rowCount);

        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                _newMatrix.matrix[i, j] = Tanh(matrix[i, j]);
            }
        }

        return _newMatrix;
    }

    // Wendet die Sigmoid Funktion auf den Wert x an
    public float Sigmoid(float x)
    {
        float y = 1 / (1 + (float)Mathf.Exp(-x));

        return y;
    }

    // Wendet die Tanh Funktion auf den Wert x an
    public float Tanh(float x)
    {
        float y = ((float)Mathf.Exp(x) - (float)Mathf.Exp(-x)) / ((float)Mathf.Exp(x) + (float)Mathf.Exp(-x));

        return y;
    }
}
