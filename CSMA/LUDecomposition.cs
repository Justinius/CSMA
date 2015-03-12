using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMA
{
    /// LU Decomposition.
    ///
    /// For an m-by-n matrix A with m >= n, the LU decomposition is an m-by-n
    /// unit lower triangular matrix L, an n-by-n upper triangular matrix U,
    /// and a permutation vector piv of length m so that A(piv,:) = L*U.
    /// If m lt n, then L is m-by-m and U is m-by-n.
    ///
    /// The LU decompostion with pivoting always exists, even if the matrix is
    ///  singular, so the constructor will never fail.  The primary use of the
    /// LU decomposition is in the solution of square systems of simultaneous
    /// linear equations.  This will fail if isNonsingular() returns false.
    public class LUDecomposition
    {

        #region Class Variables

        /// <summary>
        /// Array for internal storage of decomposition.
        /// </summary>
        private double[][] LU;
        
        
        /// <summary>
        /// Row and column dimensions, and pivot sign.
        /// </summary>
        private int m, n, pivsign;
        
        /// <summary>
        /// Internal storage of pivot vector.
        /// </summary>
        private int[] piv;

        #endregion

        /* ------------------------
        Constructor
        * ------------------------ */

        /// <summary>
        /// LU Decomposition
        /// Structure to access L, u, and piv
        /// </summary>
        /// <param name="A">A rectangular matrix</param>
        public LUDecomposition (Matrix A) 
        {
            // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

            LU = A.getArrayCopy();
            m = A.rowDimension;
            n = A.colDimension;
            piv = new int[m];
            for (int i = 0; i < m; i++) 
            {
                piv[i] = i;
            }
            pivsign = 1;
            double[] LUrowi;
            double[] LUcolj = new double[m];

            // Outer loop.
            for (int j = 0; j < n; j++) 
            {
                // Make a copy of the j-th column to localize references.
                for (int i = 0; i < m; i++) 
                {
                    LUcolj[i] = LU[i][j];
                }

                // Apply previous transformations.
                for (int i = 0; i < m; i++) 
                {
                    LUrowi = LU[i];

                    // Most of the time is spent in the following dot product.
                    int kmax = Math.Min(i,j);
                    double s = 0.0;
                    for (int k = 0; k < kmax; k++) 
                    {
                        s += LUrowi[k]*LUcolj[k];
                    }

                    LUrowi[j] = LUcolj[i] -= s;
                }
   
                // Find pivot and exchange if necessary.
                int p = j;
                for (int i = j+1; i < m; i++) 
                {
                    if (Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p])) 
                    {
                        p = i;
                    }
                }
                
                if (p != j) 
                {
                    for (int k = 0; k < n; k++) 
                    {
                        double t = LU[p][k]; LU[p][k] = LU[j][k]; LU[j][k] = t;
                    }
                    int tmp = piv[p]; piv[p] = piv[j]; piv[j] = tmp;
                    pivsign = -pivsign;
                }

                // Compute multipliers.
                if (j < m & LU[j][j] != 0.0) 
                {
                    for (int i = j+1; i < m; i++) 
                    {
                        LU[i][j] /= LU[j][j];
                    }
                }
            }
        }

        #region public methods

        /// <summary>
        /// Is the matrix nonsingular?
        /// </summary>
        /// <returns>true if U, and hence A, is nonsingular</returns>
        public bool isNonsingular () 
        {
            for (int j = 0; j < n; j++) 
            {
                if (LU[j][j] == 0)
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Return lower triangular factor
        /// </summary>
        /// <returns>L</returns>
        public Matrix getL () 
        {
            Matrix X = new Matrix(m,n);
            double[][] L = X.getArray();
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    if (i > j) 
                    {
                        L[i][j] = LU[i][j];
                    } 
                    else if (i == j) 
                    {
                        L[i][j] = 1.0;
                    } 
                    else 
                    {
                        L[i][j] = 0.0;
                    }
                }
            }
            return X;
        }

        /// <summary>
        /// Return upper triangular factor
        /// </summary>
        /// <returns>U</returns>
        public Matrix getU () 
        {
            Matrix X = new Matrix(n,n);
            double[][] U = X.getArray();
            for (int i = 0; i < n; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    if (i <= j) 
                    {
                        U[i][j] = LU[i][j];
                    } 
                    else 
                    {
                        U[i][j] = 0.0;
                    }
                }
            }
            return X;
        }
        
        /// <summary>
        /// Return pivot permuation vector
        /// </summary>
        /// <returns>piv</returns>
        public int[] getPivot () 
        {
            int[] p = new int[m];
            for (int i = 0; i < m; i++) 
            {
                p[i] = piv[i];
            }
            return p;
        }
        
        /// <summary>
        /// Return pivot permutation vector as a one-dimensional array
        /// </summary>
        /// <returns>piv</returns>
        public double[] getDoublePivot () 
        {
            double[] vals = new double[m];
            for (int i = 0; i < m; i++) 
            {
                vals[i] = piv[i];
            }
            return vals;
        }
        
        /// <summary>
        /// Determinant
        /// </summary>
        /// <returns>det(A)</returns>
        public double det ()
        {
            if (m != n)
            {
                throw new ArgumentException("Matrix must be square.");
            }
            double d = (double) pivsign;
            for (int j = 0; j < n; j++) 
            {
                d *= LU[j][j];
            }
            return d;
        }
        
        /// <summary>
        /// Solve A*X = B
        /// </summary>
        /// <param name="B">A matrix with as many rows as A and any number of columns</param>
        /// <returns>X so that L*U*X = B(piv,:)</returns>
        public Matrix solve (Matrix B) 
        {
            if (B.rowDimension != m) 
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!this.isNonsingular()) 
            {
                throw new Exception("Matrix is singular.");
            }

            // Copy right hand side with pivoting
            int nx = B.colDimension;
            Matrix Xmat = B.getMatrix(piv,0,nx-1);
            double[][] X = Xmat.getArray();

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n; k++) 
            {
                for (int i = k+1; i < n; i++) 
                {
                    for (int j = 0; j < nx; j++) 
                    {
                        X[i][j] -= X[k][j]*LU[i][k];
                    }
                }
            }
            
            // Solve U*X = Y;
            for (int k = n-1; k >= 0; k--) 
            {
                for (int j = 0; j < nx; j++) 
                {
                    X[k][j] /= LU[k][k];
                }
                for (int i = 0; i < k; i++) 
                {
                    for (int j = 0; j < nx; j++) 
                    {
                        X[i][j] -= X[k][j]*LU[i][k];
                    }
                }
            }
            return Xmat;
        }

        #endregion

        private static long serialVersionUID = 1;
    }

}
