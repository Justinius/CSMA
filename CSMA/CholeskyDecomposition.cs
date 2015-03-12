using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMA
{
       
    /// <summary>
    /// Cholesky Decomposition  
    /// 
    /// For a symmetric, positive definite matrix A, the Cholesky decomposition
    /// is an lower triangular matrix L so that A = L*L'.
    /// 
    /// If the matrix is not symmetric or positive definite, the constructor
    /// returns a partial decomposition and sets an internal flag that may
    /// be queried by the isSPD() method.
    /// </summary>
    public class CholeskyDecomposition
    {

        #region Class variables
        
        /// <summary>
        /// Array for internal storage of decomposition
        /// </summary>
        private double[][] L;
       
        /// <summary>
        /// Row and column dimension (square matrix)
        /// </summary>
        private int n;
        

        /// <summary>
        /// Symmetric and positive definite flag.
        /// </summary>
        private bool isspd;
        
        #endregion
    
        #region Constructor

        /// <summary>
        /// Cholesky algorithm for symmetric and positive definite matrix
        /// Structure to access L and isspd flag
        /// </summary>
        /// <param name="Arg">Square symmetric matrix</param>
        public CholeskyDecomposition (Matrix Arg) 
        {
            // Initialize.
            double[][] A = Arg.getArray();
            n = Arg.rowDimension;
            L = new double[n][];
            for (int i = 0; i < n; i++ )
            {
                L[i] = new double[n];
            }
            isspd = (Arg.colDimension == n);
            
            // Main loop.
            for (int j = 0; j < n; j++) 
            {
                double[] Lrowj = L[j];
                double d = 0.0;
                for (int k = 0; k < j; k++) 
                {
                    double[] Lrowk = L[k];
                    double s = 0.0;
                    for (int i = 0; i < k; i++) 
                    {
                        s += Lrowk[i]*Lrowj[i];
                    }
                    
                    Lrowj[k] = s = (A[j][k] - s)/L[k][k];
                    d = d + s*s;
                    isspd = isspd & (A[k][j] == A[j][k]); 
                }

                d = A[j][j] - d;
                isspd = isspd & (d > 0.0);
                L[j][j] = Math.Sqrt(Math.Max(d,0.0));
                for (int k = j+1; k < n; k++) 
                {
                    L[j][k] = 0.0;
                }
            }
        }

        #endregion


        #region Public methods

        /// <summary>
        /// Is the matrix symmetric and positive definite
        /// </summary>
        /// <returns>Is SPD</returns>
        public bool isSPD() 
        {
            return isspd;
        }
        
        
        /// <summary>
        /// Return triangular factor
        /// </summary>
        /// <returns>L</returns>
        public Matrix getL () 
        {
            return new Matrix(L,n,n);
        }
        
        /// <summary>
        /// Solve A*X = B
        /// </summary>
        /// <param name="B">A matrix with a many rows a A and any number of columns</param>
        /// <returns>X so that L*L'*X = B</returns>
        public Matrix solve (Matrix B) 
        {
            if (B.rowDimension != n) 
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            
            if (!isspd) 
            {
                throw new Exception("Matrix is not symmetric positive definite.");
            }

            // Copy right hand side.
            double[][] X = B.getArrayCopy();
            int nx = B.colDimension;

	        // Solve L*Y = B;
	        for (int k = 0; k < n; k++) 
            {
	            for (int j = 0; j < nx; j++) 
                {
	                for (int i = 0; i < k ; i++) 
                    {
	                    X[k][j] -= X[i][j]*L[k][i];
	                }
	                X[k][j] /= L[k][k];
	            }
	        }
	
	        // Solve L'*X = Y;
	        for (int k = n-1; k >= 0; k--) 
            {
	            for (int j = 0; j < nx; j++) 
                {
	                for (int i = k+1; i < n ; i++) 
                    {
	                    X[k][j] -= X[i][j]*L[i][k];
	                }
	                X[k][j] /= L[k][k];
	            }
	        }
      
            return new Matrix(X,n,nx);
        }
        #endregion

        private static long serialVersionUID = 1;

    }
}
