using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace CSMA
{
    public class Matrix
    {
        /// <summary>
        /// Internal data storage for matrix
        /// </summary>
        private double[][] A;

        /// <summary>
        /// Row and column dimensions
        /// </summary>
        private int m, n; //possibly want getters and setters
        public int rowDimension
        {
            get
            {
                return m;
            }
        }

        public int colDimension
        {
            get
            {
                return n;
            }
        }

        #region Contructors
        
        /// <summary>
        /// Construct an m-by-n matrix of zeros
        /// </summary>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        public Matrix (int m, int n) 
        {
            this.m = m;
            this.n = n;
            initInternalRep(m, n);
        }

        /// <summary>
        /// Construct an m-by-n constant matrix
        /// </summary>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        /// <param name="s">Constant value</param>
        public Matrix (int m, int n, double s)
        {
            this.m = m;
            this.n = n;
            initInternalRep(m,n);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i][j] = s;
                }
            }
        }
        
        /// <summary>
        /// Constructor from 2D jagged array
        /// </summary>
        /// <param name="A">2D jagged array of doubles</param>
        public Matrix (double[][] A)
        {
            m = A.Length;
            n = A[0].Length;
            for (int i = 0; i < m; i++) 
            {
                if (A[i].Length != n) 
                {
                    throw new ArgumentException("All rows must have the same length.");
                }
            }
            this.A = A;
        }
       
        /// <summary>
        /// Construct a matrix without error checking from 2D jagged array
        /// </summary>
        /// <param name="A">2D jagged array of doubles</param>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        public Matrix (double[][] A, int m, int n) 
        {
            this.A = A;
            this.m = m;
            this.n = n;
        }
                

   
        /// <summary>
        /// Construct a matrix from a one-dimensional packed array
        /// </summary>
        /// <param name="vals">One dimensional array of doubles, packed by columns (ala Fortran)</param>
        /// <param name="m">Number of rows</param>
        public Matrix (double[] vals, int m)
        {
            this.m = m;
            n = (m != 0 ? vals.Length/m : 0);
            
            if (m*n != vals.Length) 
            {
                throw new ArgumentException("Array length must be a multiple of m.");
            }
            
            initInternalRep(m,n);
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    A[i][j] = vals[i+j*m];
                }
            }
        }
        
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Construct a matrix from a copy of a 2D jagged array
        /// </summary>
        /// <param name="A">2D jagged array of dobules</param>
        /// <returns>Matrix with values copied from A</returns>
        public static Matrix constructWithCopy(double[][] A) 
        {
            int m = A.Length;
            int n = A[0].Length;
            Matrix X = new Matrix(m,n);
            double[][] C = X.getArray();
            for (int i = 0; i < m; i++) 
            {
                if (A[i].Length != n) 
                {
                    throw new ArgumentException("All rows must have the same length.");
                }
                for (int j = 0; j < n; j++) 
                {
                    C[i][j] = A[i][j];
                }
            }
            return X;
        }

        /// <summary>
        /// Make a deep copy of a matrix
        /// </summary>
        /// <returns>Deep copy of matrix</returns>
        public Matrix copy () 
        {
            Matrix X = new Matrix(m,n);
            double[][] C = X.getArray();
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    C[i][j] = A[i][j];
                }
            }
            return X;
        }
        
        /// <summary>
        /// Access the internal 2D jagged array
        /// </summary>
        /// <returns>2D jagged array of matrix elements</returns>
        public double[][] getArray () 
        {
            return A;
        }
        
        /// <summary>
        /// Copy internal two-dimensional array
        /// </summary>
        /// <returns>Copy of 2D array of matrix elements</returns>
        public double[][] getArrayCopy () 
        {
            double[][] C = new double[m][];
            for (int i = 0; i < m; i++) 
            {
                C[i] = new double[n];
                for (int j = 0; j < n; j++) 
                {
                    C[i][j] = A[i][j];
                }
            }
            return C;
        }
        
        /// <summary>
        /// Makes a 1D column packed copy of the internal array
        /// </summary>
        /// <returns>Matrix elements packed in a 1D array by columns</returns>
        public double[] getColumnPackedCopy () 
        {
            double[] vals = new double[m*n];
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    vals[i+j*m] = A[i][j];
                }
            }
            return vals;
        }
        
        /// <summary>
        /// Creates a 1D row packed copy of the internal array
        /// </summary>
        /// <returns>Matrix elements packed in a 1D array by rows</returns>
        public double[] getRowPackedCopy () 
        {
            double[] vals = new double[m*n];
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    vals[i*n+j] = A[i][j];
                }
            }
            return vals;
        }
        
        /// <summary>
        /// Get an element from the matrix
        /// </summary>
        /// <param name="i">Row index</param>
        /// <param name="j">Column inded</param>
        /// <returns>A[i][j]</returns>
        public double this [int i, int j]
        {
            get
            {
                return A[i][j];
            }

            set
            {
                A[i][j] = value;
            }
        }

        /// <summary>
        /// Get submatrix
        /// </summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <returns>A(i0:i1, j0:j1) as a matrix</returns>
        public Matrix getMatrix (int i0, int i1, int j0, int j1) 
        {
            Matrix X = new Matrix(i1-i0+1,j1-j0+1);
            double[][] B = X.getArray();
            
            try 
            {
                for (int i = i0; i <= i1; i++) 
                {
                    for (int j = j0; j <= j1; j++) 
                    {
                        B[i-i0][j-j0] = A[i][j];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
            
            return X;
        }
        
        /// <summary>
        /// Get submatrix
        /// </summary>
        /// <param name="r">Array of row indices</param>
        /// <param name="c">Array of column indices</param>
        /// <returns>A(r(:),c(:))</returns>
        public Matrix getMatrix (int[] r, int[] c) 
        {
            Matrix X = new Matrix(r.Length,c.Length);
            double[][] B = X.getArray();
            try {
                for (int i = 0; i < r.Length; i++) 
                {
                    for (int j = 0; j < c.Length; j++) 
                    {
                        B[i][j] = A[r[i]][c[j]];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
            return X;
        }
        
        /// <summary>
        /// Get submatrix
        /// </summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="c">Array of column indices</param>
        /// <returns>A(i0:i1, c(:))</returns>
        public Matrix getMatrix (int i0, int i1, int[] c)
        {
            Matrix X = new Matrix(i1-i0+1,c.Length);
            double[][] B = X.getArray();
            try 
            {
                for (int i = i0; i <= i1; i++) 
                {
                    for (int j = 0; j < c.Length; j++) 
                    {
                        B[i-i0][j] = A[i][c[j]];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
            return X;
        }

        /// <summary>
        /// Get submatrix
        /// </summary>
        /// <param name="r">Array of row indices</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <returns>A(r(:),j0:j1)</returns>
        public Matrix getMatrix (int[] r, int j0, int j1) 
        {
            Matrix X = new Matrix(r.Length,j1-j0+1);
            double[][] B = X.getArray();
            try 
            {
                for (int i = 0; i < r.Length; i++) 
                {
                    for (int j = j0; j <= j1; j++) 
                    {
                        B[i][j-j0] = A[r[i]][j];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
            return X;
        }
         
        /// <summary>
        /// Set submatrix
        /// </summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <param name="X"> A(i0:i1, j0:j1)</param>
        public void setMatrix (int i0, int i1, int j0, int j1, Matrix X) 
        {
            try 
            {
                for (int i = i0; i <= i1; i++) 
                {
                    for (int j = j0; j <= j1; j++) 
                    {
                        A[i][j] = X[i-i0,j-j0];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
        }
        
        /// <summary>
        /// Set submatrix
        /// </summary>
        /// <param name="r">Array of row indices</param>
        /// <param name="c">Array of column indices</param>
        /// <param name="X">A(r(:), c(:))</param>
        public void setMatrix (int[] r, int[] c, Matrix X) 
        {
            try 
            {
                for (int i = 0; i < r.Length; i++) 
                {
                    for (int j = 0; j < c.Length; j++) 
                    {
                        A[r[i]][c[j]] = X[i,j];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
        }
       
        /// <summary>
        /// Set submatrix
        /// </summary>
        /// <param name="r">Array of Row Indices</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <param name="X">A(r(:), j0:j1)</param>
        public void setMatrix (int[] r, int j0, int j1, Matrix X) 
        {
            try 
            {
                for (int i = 0; i < r.Length; i++) 
                {
                    for (int j = j0; j <= j1; j++) 
                    {
                        A[r[i]][j] = X[i,j-j0];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
        }
       
        /// <summary>
        /// Set submatrix
        /// </summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="c">Array of column indices</param>
        /// <param name="X">A(i0:i1, c(:))</param>
        public void setMatrix (int i0, int i1, int[] c, Matrix X) 
        {
            try 
            {
                for (int i = i0; i <= i1; i++) 
                {
                    for (int j = 0; j < c.Length; j++) 
                    {
                        A[i][c[j]] = X[i-i0,j];
                    }
                }
            } 
            catch(IndexOutOfRangeException e) 
            {
                throw new IndexOutOfRangeException("Submatrix indices");
            }
        }
        
        /// <summary>
        /// Matrix transpose
        /// </summary>
        /// <returns>A'</returns>
        public Matrix transpose () 
        {
            Matrix X = new Matrix(n,m);
            double[][] C = X.getArray();
            
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    C[j][i] = A[i][j];
                }
            }
            return X;
        }
        
        /// <summary>
        /// One norm
        /// </summary>
        /// <returns>Maximum Column Sum</returns>
        public double norm1 () 
        {
            double f = 0;
            for (int j = 0; j < n; j++) 
            {
                double s = 0;
                for (int i = 0; i < m; i++) 
                {
                    s += Math.Abs(A[i][j]);
                }
                f = Math.Max(f,s);
            }
            return f;
        }
        
        /// <summary>
        /// 2 Norm
        /// </summary>
        /// <returns>Maximum Singular value</returns>
        public double norm2 () 
        {
            return (new SingularValueDecomposition(this).norm2());
        }
       
        /// <summary>
        /// Infinity Norm
        /// </summary>
        /// <returns>Maximum row sum</returns>
        public double normInf () 
        {
            double f = 0;
            for (int i = 0; i < m; i++) 
            {
                double s = 0;
                for (int j = 0; j < n; j++) 
                {
                    s += Math.Abs(A[i][j]);
                }
                f = Math.Max(f,s);
            }
            return f;
        }
        
        /// <summary>
        /// Frobenius Norm
        /// </summary>
        /// <returns>Sqrt of sum of squares of all elements</returns>
        public double normF () 
        {
            double f = 0;
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    f = Maths.Hypot(f,A[i][j]);
                }
            }
            return f;
        }
         
        /// <summary>
        /// Unary Minus
        /// </summary>
        /// <returns>-A</returns>
        public static Matrix operator -(Matrix M) 
        {
            Matrix X = new Matrix(M.m,M.n);
            double[][] C = X.getArray();
            for (int i = 0; i < M.m; i++) 
            {
                for (int j = 0; j < M.n; j++) 
                {
                    C[i][j] = -M.A[i][j];
                }
            }
            return X;
        }
          
        /// <summary>
        /// Plus operator
        /// </summary>
        /// <param name="A">First matrix to add</param>
        /// <param name="B">Second matrix to add</param>
        /// <returns>C = M + B</returns>
        public static Matrix operator +(Matrix A, Matrix B) 
        {
            checkMatrixDimensions(A, B);
            Matrix X = new Matrix(A.m,A.n);
            double[][] C = X.getArray();
            for (int i = 0; i < A.m; i++) 
            {
                for (int j = 0; j < A.n; j++) 
                {
                    C[i][j] = A.A[i][j] + B.A[i][j];
                }
            }
            return X;
        }
        
        /// <summary>
        /// Matrix minus
        /// </summary>
        /// <param name="A">First operand</param>
        /// <param name="B">Matrix to subtract from A</param>
        /// <returns>C = A - B</returns>
        public static Matrix operator -(Matrix A, Matrix B) 
        {
            checkMatrixDimensions(A, B);
            Matrix X = new Matrix(A.m,A.n);
            double[][] C = X.getArray();
            for (int i = 0; i < A.m; i++) 
            {
                for (int j = 0; j < A.n; j++) 
                {
                    C[i][j] = A.A[i][j] - B.A[i][j];
                }
            }
            return X;
        }
      
        /// <summary>
        /// Matrix Element by Element Multiplication.
        /// Since C# does not allow custom operator to be defined this must use function call form
        /// </summary>
        /// <param name="A">First Matrix Operand</param>
        /// <param name="B">Second Matrix Operand</param>
        /// <returns>C = A .* B</returns>
        public static Matrix arrayTimes (Matrix A, Matrix B) 
        {
            checkMatrixDimensions(A, B);
            Matrix X = new Matrix(A.m,A.n);
            double[][] C = X.getArray();
            for (int i = 0; i < A.m; i++) 
            {
                for (int j = 0; j < A.n; j++) 
                {
                    C[i][j] = A.A[i][j] * B.A[i][j];
                }
            }
            return X;
        }
         
        /// <summary>
        /// Matrix element by element multiplication
        /// </summary>
        /// <param name="B">Matrix to muliply by</param>
        /// <returns>Current matrix element multiplication with B</returns>
        public Matrix arrayTimes(Matrix B)
        {
            //checkMatrixDimensions(this, B);
            //Matrix X = new Matrix(m, n);
            //double[][] C = X.getArray();
            //for (int i = 0; i < m; i++)
            //{
            //    for (int j = 0; j < n; j++)
            //    {
            //        C[i][j] = A[i][j] * B.A[i][j];
            //    }
            //}
            //return X;
            return arrayTimes(this, B);
        }

        
        /// <summary>
        /// Element by Element multiplication in place A = A.*B
        /// </summary>
        /// <param name="B">Another matrix</param>
        /// <returns>A.*B</returns>
        public Matrix arrayTimesEquals(Matrix B)
        {
            checkMatrixDimensions(this, B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i][j] = A[i][j] * B.A[i][j];
                }
            }
            return this;
        }
        
        
        /// <summary>
        /// Element division of matrices
        /// The original JAMA code had both left and right divide
        /// Since no operators can be added only the / operator is overloaded
        /// Order the matrices appropriately
        /// In the JAMA package this is called Right Division
        /// </summary>
        /// <param name="A">Matrix to be divided element-wise</param>
        /// <param name="B">Matrix to divide with element-wise</param>
        /// <returns>C = A ./ B</returns>
        public static Matrix operator /(Matrix A, Matrix B) 
        {
            checkMatrixDimensions(A, B);
            Matrix X = new Matrix(A.m,A.n);
            double[][] C = X.getArray();
            for (int i = 0; i < A.m; i++)
            {
                for (int j = 0; j < A.n; j++) 
                {
                    C[i][j] = A.A[i][j] / B.A[i][j];
                }
            }
            return X;
        }

        /// <summary>
        /// Element by element left divions, C = A.\B
        /// </summary>
        /// <param name="B">Antoher matrix</param>
        /// <returns>A.\B</returns>
        public Matrix arrayLeftDivide(Matrix B)
        {
            //checkMatrixDimensions(this, B);
            //Matrix X = new Matrix(m, n);
            //double[][] C = X.getArray();
            //for (int i = 0; i < m; i++)
            //{
           //     for (int j = 0; j < n; j++)
           //     {
           //         C[i][j] = B.A[i][j] / A[i][j];
           //     }
           // }
           // return X;
            return B / this;
        }
        
        /// <summary>
        /// Element by elment left divion in place A = A.\B
        /// </summary>
        /// <param name="B">Another matrix</param>
        /// <returns>this = A.\B</returns>
        public Matrix arrayLeftDivideEquals(Matrix B)
        {
            checkMatrixDimensions(this, B);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i][j] = B.A[i][j] / A[i][j];
                }
            }
            return this;
        }
        

        /// <summary>
        /// Scalar Matrix Multiplication
        /// </summary>
        /// <param name="s">Scalar to multiply by</param>
        /// <param name="A">Matrix to multiply</param>
        /// <returns>C = s * A</returns>
        public static Matrix operator *(double s, Matrix A) 
        {
            Matrix X = new Matrix(A.m,A.n);
            double[][] C = X.getArray();
            for (int i = 0; i < A.m; i++) 
            {
                for (int j = 0; j < A.n; j++) 
                {
                    C[i][j] = s*A.A[i][j];
                }
            }
            return X;
        }
        
        /// <summary>
        /// Scalar Matrix Multiplication
        /// </summary>
        /// <param name="A">Matrix to multiply</param>
        /// <param name="s">Scalar to multiply by</param>
        /// <returns>C = A * s</returns>
        public static Matrix operator *(Matrix A, double s)
        {
            return s*A;
        }

        /// <summary>
        /// Linear algebraic matrix multiplication, A * B
        /// Matrix multiplication is not generally commutative. Use parens and correct order
        /// </summary>
        /// <param name="A">First Matrix</param>
        /// <param name="B">Second Matrix</param>
        /// <returns></returns>
        public static Matrix operator *(Matrix A, Matrix B) 
        {
            if (B.m != A.n) 
            {
                throw new ArgumentException("Matrix inner dimensions must agree.");
            }
            Matrix X = new Matrix(A.m,B.n);
            double[][] C = X.getArray();
            double[] Bcolj = new double[A.n];
            
            for (int j = 0; j < B.n; j++)
            {
                for (int k = 0; k < A.n; k++)
                {
                    Bcolj[k] = B.A[k][j];
                }
                
                for (int i = 0; i < A.m; i++) 
                {
                    double[] Arowi = A.A[i];
                    double s = 0;
                    for (int k = 0; k < A.n; k++) 
                    {
                        s += Arowi[k]*Bcolj[k];
                    }
                    C[i][j] = s;
                }
            }
            return X;
        }

        /// <summary>
        /// Linear algebraic matrix multiplication A * B
        /// This is implemented same as the overloaded operator, just referncing 
        /// current Matrix for stringing commands
        /// </summary>
        /// <param name="B">Another matrix</param>
        /// <returns>Matrix Product, this * B</returns>
        public Matrix times(Matrix B)
        {
            //if (B.m != n)
            //{
            //    throw new ArgumentException("Matrix inner dimensions must agree.");
            //}
            //Matrix X = new Matrix(m, B.n);
            //double[][] C = X.getArray();
            //double[] Bcolj = new double[n];

            //for (int j = 0; j < B.n; j++)
            //{
            //    for (int k = 0; k < n; k++)
            //    {
            //        Bcolj[k] = B.A[k][j];
            //    }

            //    for (int i = 0; i < m; i++)
            //    {
            //        double[] Arowi = A[i];
            //        double s = 0;
            //        for (int k = 0; k < n; k++)
            //        {
            //            s += Arowi[k] * Bcolj[k];
            //        }
            //        C[i][j] = s;
            //    }
            //}
            //return X;
            return this * B;

        }

        #region Dependent on other classes
   
        /// <summary>
        /// LU Decomposition
        /// See LUDecomposition for more information
        /// </summary>
        /// <returns>LU Decomposition of this matrix</returns>
        public LUDecomposition lu () 
        {
            return new LUDecomposition(this);
        }

        /// <summary>
        /// QR Decomposition
        /// See QRDecomposition for more information
        /// </summary>
        /// <returns>QR Decomposition object of this</returns>
        public QRDecomposition qr () 
        {
            return new QRDecomposition(this);
        }

        /// <summary>
        /// Cholesky Decomposition
        /// See CholeskyDecomposition for more information
        /// </summary>
        /// <returns>Cholesky Decomposition object of this</returns>
        public CholeskyDecomposition chol () 
        {
            return new CholeskyDecomposition(this);
        }
        
        /// <summary>
        /// Singular Value Decomposition
        /// See SingularValueDecomposition for more information
        /// </summary>
        /// <returns>Singular Value Decomposition object of this</returns>
        public SingularValueDecomposition svd () 
        {
            return new SingularValueDecomposition(this);
        }
        
        /// <summary>
        /// Eigen Value Decomposition
        /// See EigenValueDecomposition for more information
        /// </summary>
        /// <returns>Eigen Value Decomposition object of this</returns>
        public EigenvalueDecomposition eig ()
        {
            return new EigenvalueDecomposition(this);
        }
       
        /// <summary>
        /// Solves A*X = B
        /// </summary>
        /// <param name="B">Right hand side</param>
        /// <returns>Solution if A is square, least squares solution otherwise</returns>
        public Matrix solve (Matrix B) 
        {
            return (m == n ? (new LUDecomposition(this)).solve(B) :
                             (new QRDecomposition(this)).solve(B));
        }
        
        /// <summary>
        /// Solve X*A = B which is also A'*X' = B'
        /// </summary>
        /// <param name="B">Right hand side</param>
        /// <returns>Solution if A is square, least squares solution otherwise</returns>
        public Matrix solveTranspose (Matrix B) 
        {
            return transpose().solve(B.transpose());
        }
        
        /// <summary>
        /// Matrix inverse of psuedoinverse
        /// </summary>
        /// <returns>Inverse(A) if A is square, psuedoinverse otherwise</returns>
        public Matrix inverse () 
        {
            return solve(identity(m,m));
        }
        
        /// <summary>
        /// Matrix Determinant
        /// </summary>
        /// <returns>Determinant</returns>
        public double det () 
        {
            return new LUDecomposition(this).det();
        }
     
        /// <summary>
        /// Matrix rank
        /// </summary>
        /// <returns>Effective numerical rank, obtained from SVD</returns>
        public int rank () 
        {
            return new SingularValueDecomposition(this).rank();
        }
        
        /// <summary>
        /// Matrix condition (2 norm)
        /// </summary>
        /// <returns>Ratio of largest to smallest singular value</returns>
        public double cond () 
        {
            return new SingularValueDecomposition(this).cond();
        }
                
        /// <summary>
        /// Matrix trace
        /// </summary>
        /// <returns>Sum of diagonal elements</returns>
        public double trace () 
        {
            double t = 0;
            for (int i = 0; i < Math.Min(m,n); i++) 
            {
                t += A[i][i];
            }
            return t;
        }
       
        /// <summary>
        /// Generate matrix with Random elements
        /// </summary>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        /// <returns>An m-by-n matrix with uniformly distributed random elements</returns>
        public static Matrix random (int m, int n) 
        {
            Random rnd = new Random();
      
            Matrix A = new Matrix(m,n);
            double[][] X = A.getArray();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) 
                {
                    X[i][j] = rnd.NextDouble();
                }
            }
            return A;
        }

        /// <summary>
        /// Generate identity matrix
        /// </summary>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        /// <returns>An m-by-n matrix with ones on the diagonal and zeros elsewhere.</returns>
        public static Matrix identity (int m, int n) 
        {
            Matrix A = new Matrix(m,n);
            double[][] X = A.getArray();
            for (int i = 0; i < m; i++) 
            {
                for (int j = 0; j < n; j++) 
                {
                    X[i][j] = (i == j ? 1.0 : 0.0);
                }
            }
            return A;
        }
   
        public string toFormatString(int width, int numDecimals)
        {
            DoubleFormatter dblF = new DoubleFormatter(width, numDecimals);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            for(int i = 0; i < m; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    string s = string.Format(dblF, "{0}", A[i][j]);
                    int padding = Math.Max(1, width-s.Length); //at least 1 space
                    for(int k = 0; k < padding; k++)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(s);
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            return sb.ToString();
        }
                
        #endregion

        #endregion

        #region private methods
        /// <summary>
        /// This prepares the memory space for the internal matrix representation
        /// This pattern is used enough I thought it would be good to extract it away
        /// Note: This may become a "generic" jagged array initializer.
        /// </summary>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        private void initInternalRep(int m, int n)
        {
            A = new double[m][];
            for(int i = 0; i < m; i++)
            {
                A[i] = new double[n];
            }
        }

       

        /// <summary>
        /// Check if size(A) == size(B)
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        private static void checkMatrixDimensions (Matrix A, Matrix B) 
        {
            if (B.m != A.m || B.n != A.n) 
            {
                throw new ArgumentException("Matrix dimensions must agree.");
            }
        }
        

        private static long serialVersionUID = 1;

        #endregion
    }

    public class Maths
    {
        /// <summary>
        /// Calculates sqrt(a^2 + b^2) without over/under flow
        /// </summary>
        /// <returns>sqrt(a^2 + b^2)</returns>
        public static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b)) 
            {
                r = b/a;
                r = Math.Abs(a)*Math.Sqrt(1+r*r);
            } 
            else if (b != 0) 
            {
                r = a/b;
                r = Math.Abs(b)*Math.Sqrt(1+r*r);
            } 
            else 
            {
                r = 0.0;
            }
            return r;
        }
    }
    
    /// <summary>
    /// Class to provide formatting for the printing options
    /// </summary>
    public class DoubleFormatter : IFormatProvider, ICustomFormatter
    {
        // always use dot separator for doubles
        private CultureInfo enUsCulture = new CultureInfo("en-US");
        private string formatStrOpen = "{0:";
        private string formatStrClose = "}";
        private string formatStr = "";
        
        private int numDecimalPlaces = 0;
        private int width = 0;


        public DoubleFormatter(int widthIn, int numDecimalPlacesIn)
        {
            if(numDecimalPlacesIn > 0)
            {
                numDecimalPlaces = numDecimalPlacesIn;
            }
            else
            {
                numDecimalPlaces = 0;
            }

            if(widthIn > numDecimalPlaces + 1)
            {
                width = widthIn;
            }
            else
            {
                width = numDecimalPlaces + 1;
            }

            formatStr = formatStrOpen;
            for (int i = 0; i < (width - numDecimalPlaces - 1); i++ )
            {
                formatStr += "0";
            }
            formatStr += ".";
            for(int i = 0; i < numDecimalPlaces; i++)
            {
                formatStr += "0";
            }
            formatStr += formatStrClose;
            
        }


        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is double)
            {
                if (string.IsNullOrEmpty(format))
                {
                    string retString = string.Format(enUsCulture, formatStr, arg);
                    return retString.PadLeft(width);
                }
                else
                {
                    // if user supplied own format use it
                    return ((double)arg).ToString(format, enUsCulture);
                }
            }
            
            // format everything else normally
            if (arg is IFormattable)
              return ((IFormattable)arg).ToString(format, formatProvider);
            else
              return arg.ToString();
        
        }
 
        public object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }
    }
}
