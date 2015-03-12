using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMA
{

    /// <summary>
    /// For an m-by-n matrix A with m >= n, the QR decomposition is an m-by-n
    /// orthogonal matrix Q and an n-by-n upper triangular matrix R so that
    ///A = Q*R.
    ///
    /// The QR decompostion always exists, even if the matrix does not have
    /// full rank, so the constructor will never fail.  The primary use of the
    /// QR decomposition is in the least squares solution of nonsquare systems
    /// of simultaneous linear equations.  This will fail if isFullRank()
    /// returns false.
    /// </summary>
    public class QRDecomposition
    {

        #region Class variables
       
        /// <summary>
        /// Array for internal storage of decoposition
        /// </summary>
        private double[][] QR;
        
        /// <summary>
        /// Row and column dimensions
        /// </summary>
        private int m, n;
        
        
        /// <summary>
        /// Array for internal storage of diagonal of R
        /// </summary>
        private double[] Rdiag;
        #endregion

        /// <summary>
        /// QR Decomposition, computed by Householder reflections
        /// Structure to access R and the Householder vectors and computer Q
        /// </summary>
        /// <param name="A">Rectangular matrix</param>
        public QRDecomposition (Matrix A) 
        {
            // Initialize.
            QR = A.getArrayCopy();
            m = A.rowDimension; 
            n = A.colDimension;
            Rdiag = new double[n];

            // Main loop.
            for (int k = 0; k < n; k++) 
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (int i = k; i < m; i++) 
                {
                    nrm = Maths.Hypot(nrm,QR[i][k]);
                }

                if (nrm != 0.0) 
                {
                    // Form k-th Householder vector.
                    if (QR[k][k] < 0) 
                    {
                        nrm = -nrm;
                    }

                    for (int i = k; i < m; i++) 
                    {
                        QR[i][k] /= nrm;
                    }
                    QR[k][k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k+1; j < n; j++) 
                    {
                        double s = 0.0; 
                        for (int i = k; i < m; i++) 
                        {
                            s += QR[i][k]*QR[i][j];
                        }
                        s = -s/QR[k][k];
                        
                        
                        for (int i = k; i < m; i++) 
                        {
                            QR[i][j] += s*QR[i][k];
                        }
                    }
                }
                Rdiag[k] = -nrm;
            }
        }

        #region public methods
        
        /// <summary>
        /// Is the matrix full rank?
        /// </summary>
        /// <returns>True if R, and hence A, has full rank</returns>
        public bool isFullRank () 
        {
            for (int j = 0; j < n; j++) 
            {
                if (Rdiag[j] == 0)
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Return the Householder vectors
        /// </summary>
        /// <returns>Lower trapezoidal matrix whose columns define the reflections</returns>
        public Matrix getH () 
        {
            Matrix X = new Matrix(m,n);
            double[][] H = X.getArray();
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    if (i >= j) 
                    {
                        H[i][j] = QR[i][j];
                    } 
                    else 
                    {
                        H[i][j] = 0.0;
                    }
                }
            }
            return X;
        }
        
        /// <summary>
        /// Return the upper triangular factor
        /// </summary>
        /// <returns>R</returns>
        public Matrix getR () 
        {
            Matrix X = new Matrix(n,n);
            double[][] R = X.getArray();
            for (int i = 0; i < n; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    if (i < j) 
                    {
                        R[i][j] = QR[i][j];
                    } 
                    else if (i == j) 
                    {
                        R[i][j] = Rdiag[i];
                    } 
                    else 
                    {
                        R[i][j] = 0.0;
                    }
                }
            }
            return X;
        }
        
        /// <summary>
        /// Generate and return the (economy-sized orthogonal factor
        /// </summary>
        /// <returns>Q</returns>
        public Matrix getQ () 
        {
            Matrix X = new Matrix(m,n);
            double[][] Q = X.getArray();
            for (int k = n-1; k >= 0; k--) 
            {
                for (int i = 0; i < m; i++) 
                {
                    Q[i][k] = 0.0;
                }
                Q[k][k] = 1.0;
                
                for (int j = k; j < n; j++) 
                {
                    if (QR[k][k] != 0) 
                    {
                        double s = 0.0;
                        for (int i = k; i < m; i++) 
                        {
                            s += QR[i][k]*Q[i][j];
                        }
                        s = -s/QR[k][k];
                        
                        for (int i = k; i < m; i++) 
                        {
                            Q[i][j] += s*QR[i][k];
                        }
                    }
                }
            }
            return X;
        }

        /// <summary>
        /// Least squares solution of A*X = B
        /// </summary>
        /// <param name="B">A matrix with as many rows as A and any number of columns</param>
        /// <returns>X that minimizes the two norm of Q*R*X-B</returns>
        public Matrix solve (Matrix B) 
        {
            if (B.rowDimension != m) 
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!this.isFullRank()) 
            {
                throw new Exception("Matrix is rank deficient.");
            }
      
            // Copy right hand side
            int nx = B.colDimension;
            double[][] X = B.getArrayCopy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++) 
            {
                for (int j = 0; j < nx; j++) 
                {
                    double s = 0.0; 
                    for (int i = k; i < m; i++) 
                    {
                        s += QR[i][k]*X[i][j];
                    }
                    s = -s/QR[k][k];
                    
                    for (int i = k; i < m; i++) 
                    {
                        X[i][j] += s*QR[i][k];
                    }   
                }
            }
            // Solve R*X = Y;
            for (int k = n-1; k >= 0; k--) 
            {
                for (int j = 0; j < nx; j++) 
                {
                    X[k][j] /= Rdiag[k];
                }
                
                for (int i = 0; i < k; i++) 
                {
                    for (int j = 0; j < nx; j++) 
                    {
                        X[i][j] -= X[k][j]*QR[i][k];
                    }
                }
            }
            return (new Matrix(X,n,nx).getMatrix(0,n-1,0,nx-1));
        }

        #endregion

        private static long serialVersionUID = 1;
    }

}
