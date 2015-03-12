using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMA;

namespace CSMATest
{
    class Program
    {
        ///TestMatrix tests the functionality of the Jama Matrix class and associated decompositions.
        ///
        ///Run the test from the command line using
        ///<BLOCKQUOTE><PRE><CODE>
        /// java Jama.test.TestMatrix 
        ///</CODE></PRE></BLOCKQUOTE>
        ///Detailed output is provided indicating the functionality being tested
        ///and whether the functionality is correctly implemented.   Exception handling
        ///is also tested.  
        ///
        ///The test is designed to run to completion and give a summary of any implementation errors
        ///encountered. The final output should be:
        ///<BLOCKQUOTE><PRE><CODE>
        ///      TestMatrix completed.
        ///      Total errors reported: n1
        ///      Total warning reported: n2
        ///</CODE></PRE></BLOCKQUOTE>
        ///If the test does not run to completion, this indicates that there is a 
        ///substantial problem within the implementation that was not anticipated in the test design.  
        ///The stopping point should give an indication of where the problem exists.
        public static void Main (String[] args) 
        {
            Matrix A,B,C,Z,O,I,R,S,X,SUB,M,T,SQ,DEF,SOL;
            // Uncomment this to test IO in a different locale.
            // Locale.setDefault(Locale.GERMAN);
            int errorCount=0;
            int warningCount=0;
            double tmp, s;
            double[] columnwise = new double[]{1.0,2.0,3.0,4.0,5.0,6.0,7.0,8.0,9.0,10.0,11.0,12.0};
            double[] rowwise = new double[]{1.0,4.0,7.0,10.0,2.0,5.0,8.0,11.0,3.0,6.0,9.0,12.0};
            double[][] avals = {new double[]{1.0,4.0,7.0,10.0},new double[]{2.0,5.0,8.0,11.0},new double[]{3.0,6.0,9.0,12.0}};
            double[][] rankdef = avals;
            double[][] tvals =  {
                                    new double[]{1.0,2.0,3.0},
                                    new double[]{4.0,5.0,6.0},
                                    new double[]{7.0,8.0,9.0},
                                    new double[]{10.0,11.0,12.0}
                                };
            double[][] subavals = {
                                      new double[]{5.0,8.0,11.0},
                                      new double[]{6.0,9.0,12.0}
                                  };
            double[][] rvals = {
                                   new double[]{1.0,4.0,7.0},
                                   new double[]{2.0,5.0,8.0,11.0},
                                   new double[]{3.0,6.0,9.0,12.0}
                               };
            double[][] pvals = {
                                   new double[]{4.0,1.0,1.0},
                                   new double[]{1.0,2.0,3.0},
                                   new double[]{1.0,3.0,6.0}
                               };
            double[][] ivals = {
                                   new double[]{1.0,0.0,0.0,0.0},
                                   new double[]{0.0,1.0,0.0,0.0},
                                   new double[]{0.0,0.0,1.0,0.0}
                               };
            double[][] evals = {
                                   new double[]{0.0,1.0,0.0,0.0},
                                   new double[]{1.0,0.0,2.0e-7,0.0},
                                   new double[]{0.0,-2.0e-7,0.0,1.0},
                                   new double[]{0.0,0.0,1.0,0.0}
                               };
            double[][] square = {
                                    new double[]{166.0,188.0,210.0},
                                    new double[]{188.0,214.0,240.0},
                                    new double[]{210.0,240.0,270.0}
                                };
            double[][] sqSolution = {
                                        new double[]{13.0},
                                        new double[]{15.0}
                                    };
            double[][] condmat = {
                                     new double[]{1.0,3.0},
                                     new double[]{7.0,9.0}
                                 };
            double[][] badeigs = {
                                     new double[]{0,0,0,0,0}, 
                                     new double[]{0,0,0,0,1},
                                     new double[]{0,0,0,1,0},
                                     new double[]{1,1,0,0,1},
                                     new double[]{1,0,1,0,1}
                                 };
            int rows=3,cols=4;
            int invalidld=5;/* should trigger bad shape for construction with val */
            int raggedr=0; /* (raggedr,raggedc) should be out of bounds in ragged array */
            int raggedc=4; 
            int validld=3; /* leading dimension of intended test Matrices */
            int nonconformld=4; /* leading dimension which is valid, but nonconforming */
            int ib=1,ie=2,jb=1,je=3; /* index ranges for sub Matrix */
            int[] rowindexset = {1,2}; 
            int[] badrowindexset = {1,3}; 
            int[] columnindexset = {1,2,3};
            int[] badcolumnindexset = {1,2,4};
            double columnsummax = 33.0;
            double rowsummax = 30.0;
            double sumofdiagonals = 15;
            double sumofsquares = 650;

            /** 
            Constructors and constructor-like methods:
                double[], int
                double[][]  
                int, int
                int, int, double
                int, int, double[][]
                constructWithCopy(double[][])
                random(int,int)
                identity(int)
            **/

            print("\nTesting constructors and constructor-like methods...\n");
            try
            {  
                /** check that exception is thrown in packed constructor with invalid length **/
                A = new Matrix(columnwise,invalidld);
                errorCount = try_failure(errorCount,"Catch invalid length in packed constructor... ",
                            "exception not thrown for invalid input");
            } 
            catch( ArgumentException e ) 
            {
                try_success("Catch invalid length in packed constructor... ", e.Message);
            }
          
            try
            { 
                /** check that exception is thrown in default constructor
                    if input array is 'ragged' **/
                A = new Matrix(rvals);
                tmp = A[raggedr,raggedc];
            } 
            catch ( ArgumentException e ) 
            {
                try_success("Catch ragged input to default constructor... ", e.Message);
            } 
            catch ( IndexOutOfRangeException e )
            {
                errorCount = try_failure(errorCount,"Catch ragged input to constructor... ", "exception not thrown in construction...ArrayIndexOutOfBoundsException thrown later");
            }
          
            try
            { 
                /** check that exception is thrown in constructWithCopy
                    if input array is 'ragged' **/
                A = Matrix.constructWithCopy(rvals);
                tmp = A[raggedr,raggedc];
            } 
            catch ( ArgumentException e ) 
            {
                try_success("Catch ragged input to constructWithCopy... ",e.Message);
            } 
            catch ( IndexOutOfRangeException e ) 
            {
                errorCount = try_failure(errorCount,"Catch ragged input to constructWithCopy... ","exception not thrown in construction...ArrayIndexOutOfBoundsException thrown later");
            }

          A = new Matrix(columnwise,validld);
          B = new Matrix(avals);
          tmp = B[0,0];
          avals[0][0] = 0.0;
          C = B - A;
          avals[0][0] = tmp;
          B = Matrix.constructWithCopy(avals);
          tmp = B[0,0];
          avals[0][0] = 0.0;
          if ( ( tmp - B[0,0] ) != 0.0 ) {
            /** check that constructWithCopy behaves properly **/
            errorCount = try_failure(errorCount,"constructWithCopy... ","copy not effected... data visible outside");
          } else {
            try_success("constructWithCopy... ","");
          }
          avals[0][0] = columnwise[0]; 
          I = new Matrix(ivals);
          try {
            check(I,Matrix.identity(3,4));
            try_success("identity... ","");
          } catch ( Exception e ) {
            errorCount = try_failure(errorCount,"identity... ","identity Matrix not successfully created");
          }   

    /**   
          Access Methods:
             getColumnDimension()
             getRowDimension()
             getArray()
             getArrayCopy()
             getColumnPackedCopy()
             getRowPackedCopy()
             get(int,int)
             getMatrix(int,int,int,int)
             getMatrix(int,int,int[])
             getMatrix(int[],int,int)
             getMatrix(int[],int[])
             set(int,int,double)
             setMatrix(int,int,int,int,Matrix)
             setMatrix(int,int,int[],Matrix)
             setMatrix(int[],int,int,Matrix)
             setMatrix(int[],int[],Matrix)
    **/

          print("\nTesting access methods...\n");

    /**
          Various get methods:
    **/

          B = new Matrix(avals);
          if (B.rowDimension != rows) 
          {
             errorCount = try_failure(errorCount,"getRowDimension... ","");
          } 
          else 
          {
             try_success("getRowDimension... ","");
          }
          if (B.colDimension != cols)
          {
             errorCount = try_failure(errorCount,"getColumnDimension... ","");
          } 
          else 
          {
             try_success("getColumnDimension... ","");
          }
          B = new Matrix(avals);
          double[][] barray = B.getArray();
          if ( barray != avals ) 
          {
             errorCount = try_failure(errorCount,"getArray... ","");
          }
          else
          {
             try_success("getArray... ","");
          }
          barray = B.getArrayCopy();
          if ( barray == avals ) 
          {
             errorCount = try_failure(errorCount,"getArrayCopy... ","data not (deep) copied");
          }
          try 
          {
             check(barray,avals);
             try_success("getArrayCopy... ","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"getArrayCopy... ","data not successfully (deep) copied");
          }
          double[] bpacked = B.getColumnPackedCopy();
          try 
          {
             check(bpacked,columnwise);
             try_success("getColumnPackedCopy... ","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"getColumnPackedCopy... ","data not successfully (deep) copied by columns");
          }
          bpacked = B.getRowPackedCopy();
          try
          {
             check(bpacked,rowwise);
             try_success("getRowPackedCopy... ","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"getRowPackedCopy... ","data not successfully (deep) copied by rows");
          }
          try 
          {
             tmp = B[B.rowDimension,B.colDimension-1];
             errorCount = try_failure(errorCount,"get(int,int)... ","OutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e )
          {
             try
             {
                tmp = B[B.rowDimension-1,B.colDimension];
                errorCount = try_failure(errorCount,"get(int,int)... ","OutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("get(int,int)... OutofBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"get(int,int)... ","OutOfBoundsException expected but not thrown");
          }
          try
          {
             if (B[B.rowDimension-1,B.colDimension-1] != avals[B.rowDimension-1][B.colDimension-1] )
             {
                errorCount = try_failure(errorCount,"get(int,int)... ","Matrix entry (i,j) not successfully retreived");
             } 
             else
             {
                try_success("get(int,int)... ","");
             }
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             errorCount = try_failure(errorCount,"get(int,int)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          SUB = new Matrix(subavals);
          try 
          {
             M = B.getMatrix(ib,ie+B.rowDimension+1,jb,je);
             errorCount = try_failure(errorCount,"getMatrix(int,int,int,int)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try
             {
                M = B.getMatrix(ib,ie,jb,je+B.colDimension+1);
                errorCount = try_failure(errorCount,"getMatrix(int,int,int,int)... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("getMatrix(int,int,int,int)... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int,int,int,int)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          }
          try 
          {
             M = B.getMatrix(ib,ie,jb,je);
             try 
             {
                check(SUB,M);
                try_success("getMatrix(int,int,int,int)... ","");
             } 
             catch ( Exception e )
             {
                errorCount = try_failure(errorCount,"getMatrix(int,int,int,int)... ","submatrix not successfully retreived");
             }
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int,int,int,int)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
      
          try 
          {
             M = B.getMatrix(ib,ie,badcolumnindexset);
             errorCount = try_failure(errorCount,"getMatrix(int,int,int[])... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try 
             {
                M = B.getMatrix(ib,ie+B.rowDimension+1,columnindexset);
                errorCount = try_failure(errorCount,"getMatrix(int,int,int[])... ","ArrayIndexOutOfBoundsException expected but not thrown");
             }
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("getMatrix(int,int,int[])... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int,int,int[])... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          try 
          {
             M = B.getMatrix(ib,ie,columnindexset);
             try 
             {
                check(SUB,M);
                try_success("getMatrix(int,int,int[])... ","");
             } 
             catch ( Exception e )
             {
                errorCount = try_failure(errorCount,"getMatrix(int,int,int[])... ","submatrix not successfully retreived");
             }
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int,int,int[])... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          try 
          {
             M = B.getMatrix(badrowindexset,jb,je);
             errorCount = try_failure(errorCount,"getMatrix(int[],int,int)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try 
             {
                M = B.getMatrix(rowindexset,jb,je+B.colDimension+1);
                errorCount = try_failure(errorCount,"getMatrix(int[],int,int)... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 )
             {
                try_success("getMatrix(int[],int,int)... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int[],int,int)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          try 
          {
             M = B.getMatrix(rowindexset,jb,je);
             try 
             {
                check(SUB,M);
                try_success("getMatrix(int[],int,int)... ","");
             } 
             catch ( Exception e ) 
             {
                errorCount = try_failure(errorCount,"getMatrix(int[],int,int)... ","submatrix not successfully retreived");
             }
          } 
          catch (IndexOutOfRangeException e ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int[],int,int)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          try 
          {
             M = B.getMatrix(badrowindexset,columnindexset);
             errorCount = try_failure(errorCount,"getMatrix(int[],int[])... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try
             {
                M = B.getMatrix(rowindexset,badcolumnindexset);
                errorCount = try_failure(errorCount,"getMatrix(int[],int[])... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("getMatrix(int[],int[])... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"getMatrix(int[],int[])... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          try 
          {
             M = B.getMatrix(rowindexset,columnindexset);
             try
             {
                check(SUB,M);
                try_success("getMatrix(int[],int[])... ","");
             } 
             catch (Exception e ) 
             {
                errorCount = try_failure(errorCount,"getMatrix(int[],int[])... ","submatrix not successfully retreived");
             }
          } 
          catch ( IndexOutOfRangeException e )
          {
             errorCount = try_failure(errorCount,"getMatrix(int[],int[])... ","Unexpected ArrayIndexOutOfBoundsException");
          }

    /**
          Various set methods:
    **/

          try
          {
             B[B.rowDimension,B.colDimension-1] = 0.0;
             errorCount = try_failure(errorCount,"set(int,int,double)... ","OutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try 
             {
                B[B.rowDimension-1,B.colDimension]=0.0;
                errorCount = try_failure(errorCount,"set(int,int,double)... ","OutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 )
             {
                try_success("set(int,int,double)... OutofBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"set(int,int,double)... ","OutOfBoundsException expected but not thrown");
          }
          try 
          {
             B[ib,jb] = 0.0;
             tmp = B[ib,jb];
             try 
             {
                check(tmp,0.0);
                try_success("set(int,int,double)... ","");
             } 
             catch ( Exception e ) 
             {
                errorCount = try_failure(errorCount,"set(int,int,double)... ","Matrix element not successfully set");
             }
          } 
          catch ( IndexOutOfRangeException e1) 
          {
             errorCount = try_failure(errorCount,"set(int,int,double)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          M = new Matrix(2,3,0.0);
          try 
          {
             B.setMatrix(ib,ie+B.rowDimension+1,jb,je,M);
             errorCount = try_failure(errorCount,"setMatrix(int,int,int,int,Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try 
             {
                B.setMatrix(ib,ie,jb,je+B.colDimension+1,M);
                errorCount = try_failure(errorCount,"setMatrix(int,int,int,int,Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("setMatrix(int,int,int,int,Matrix)... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"setMatrix(int,int,int,int,Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          }
          try 
          {
             B.setMatrix(ib,ie,jb,je,M);
             try 
             {
                check(M - B.getMatrix(ib,ie,jb,je),M);
                try_success("setMatrix(int,int,int,int,Matrix)... ","");
             } 
             catch ( Exception e )
             {
                errorCount = try_failure(errorCount,"setMatrix(int,int,int,int,Matrix)... ","submatrix not successfully set");
             }
             B.setMatrix(ib,ie,jb,je,SUB);
          } 
          catch ( IndexOutOfRangeException e1 )
          {
             errorCount = try_failure(errorCount,"setMatrix(int,int,int,int,Matrix)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          try 
          {
             B.setMatrix(ib,ie+B.rowDimension+1,columnindexset,M);
             errorCount = try_failure(errorCount,"setMatrix(int,int,int[],Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e )
          {
             try 
             {
                B.setMatrix(ib,ie,badcolumnindexset,M);
                errorCount = try_failure(errorCount,"setMatrix(int,int,int[],Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("setMatrix(int,int,int[],Matrix)... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"setMatrix(int,int,int[],Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          }
          try 
          {
             B.setMatrix(ib,ie,columnindexset,M);
             try 
             {
                check(M - B.getMatrix(ib,ie,columnindexset),M);
                try_success("setMatrix(int,int,int[],Matrix)... ","");
             } 
             catch ( Exception e )
             {
                errorCount = try_failure(errorCount,"setMatrix(int,int,int[],Matrix)... ","submatrix not successfully set");
             }
             B.setMatrix(ib,ie,jb,je,SUB);
          } 
          catch ( IndexOutOfRangeException e1 ) 
          {
             errorCount = try_failure(errorCount,"setMatrix(int,int,int[],Matrix)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          try 
          {
             B.setMatrix(rowindexset,jb,je+B.colDimension+1,M);
             errorCount = try_failure(errorCount,"setMatrix(int[],int,int,Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e ) 
          {
             try 
             {
                B.setMatrix(badrowindexset,jb,je,M);
                errorCount = try_failure(errorCount,"setMatrix(int[],int,int,Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 ) 
             {
                try_success("setMatrix(int[],int,int,Matrix)... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"setMatrix(int[],int,int,Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          }
          try 
          {
             B.setMatrix(rowindexset,jb,je,M);
             try 
             {
                check(M - B.getMatrix(rowindexset,jb,je),M);
                try_success("setMatrix(int[],int,int,Matrix)... ","");
             } 
             catch ( Exception e ) 
             {
                errorCount = try_failure(errorCount,"setMatrix(int[],int,int,Matrix)... ","submatrix not successfully set");
             }
             B.setMatrix(ib,ie,jb,je,SUB);
          } 
          catch ( IndexOutOfRangeException e1 ) 
          {
             errorCount = try_failure(errorCount,"setMatrix(int[],int,int,Matrix)... ","Unexpected ArrayIndexOutOfBoundsException");
          }
          try 
          {
             B.setMatrix(rowindexset,badcolumnindexset,M);
             errorCount = try_failure(errorCount,"setMatrix(int[],int[],Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          } 
          catch ( IndexOutOfRangeException e )
          {
             try 
             {
                B.setMatrix(badrowindexset,columnindexset,M);
                errorCount = try_failure(errorCount,"setMatrix(int[],int[],Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
             } 
             catch ( IndexOutOfRangeException e1 )
             {
                try_success("setMatrix(int[],int[],Matrix)... ArrayIndexOutOfBoundsException... ","");
             }
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"setMatrix(int[],int[],Matrix)... ","ArrayIndexOutOfBoundsException expected but not thrown");
          }
          try 
          {
             B.setMatrix(rowindexset,columnindexset,M);
             try 
             {
                check(M - B.getMatrix(rowindexset,columnindexset),M);
                try_success("setMatrix(int[],int[],Matrix)... ","");
             } 
             catch ( Exception e ) 
             {
                errorCount = try_failure(errorCount,"setMatrix(int[],int[],Matrix)... ","submatrix not successfully set");
             }
          } 
          catch ( IndexOutOfRangeException e1 )
          {
             errorCount = try_failure(errorCount,"setMatrix(int[],int[],Matrix)... ","Unexpected ArrayIndexOutOfBoundsException");
          }

    /** 
          Array-like methods:
             minus
             minusEquals
             plus
             plusEquals
             arrayLeftDivide
             arrayLeftDivideEquals
             arrayRightDivide
             arrayRightDivideEquals
             arrayTimes
             arrayTimesEquals
             uminus
    **/

          print("\nTesting array-like methods...\n");
          S = new Matrix(columnwise,nonconformld);
          R = Matrix.random(A.rowDimension,A.colDimension);
          A = R;
          try 
          {
            S = A - S;
            errorCount = try_failure(errorCount,"minus conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("minus conformance check... ","");
          }
          if ((A-R).norm1() != 0.0) 
          {
            errorCount = try_failure(errorCount,"minus... ","(difference of identical Matrices is nonzero,\nSubsequent use of minus should be suspect)");
          } 
          else 
          {
            try_success("minus... ","");
          }
          A = R.copy();
          A -= R;
          Z = new Matrix(A.rowDimension,A.colDimension);
          try 
          {
            A -= S;
            errorCount = try_failure(errorCount,"minusEquals conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("minusEquals conformance check... ","");
          }
          if ((A-Z).norm1() != 0.0) 
          {
            errorCount = try_failure(errorCount,"minusEquals... ","(difference of identical Matrices is nonzero,\nSubsequent use of minus should be suspect)");
          }
          else 
          {
            try_success("minusEquals... ","");
          }

          A = R.copy();
          B = Matrix.random(A.rowDimension,A.colDimension);
          C = A - B; 
          try 
          {
            S = A + S;
            errorCount = try_failure(errorCount,"plus conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("plus conformance check... ","");
          }
          try 
          {
            check(C+B,A);
            try_success("plus... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"plus... ","(C = A - B, but C + B != A)");
          }
          C = A - B;
          C += B;
          try
          {
            A += S;
            errorCount = try_failure(errorCount,"plusEquals conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("plusEquals conformance check... ","");
          }
          try 
          {
            check(C,A);
            try_success("plusEquals... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"plusEquals... ","(C = A - B, but C = C + B != A)");
          }
          A = -R;
          try 
          {
            check(A+R,Z);
            try_success("uminus... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"uminus... ","(-A + A != zeros)");
          }
          A = R.copy();
          O = new Matrix(A.rowDimension,A.colDimension,1.0);
          C = A.arrayLeftDivide(R);
          try 
          {
            S = A.arrayLeftDivide(S);
            errorCount = try_failure(errorCount,"arrayLeftDivide conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("arrayLeftDivide conformance check... ","");
          }
          try 
          {
            check(C,O);
            try_success("arrayLeftDivide... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"arrayLeftDivide... ","(M.\\M != ones)");
          }
          try 
          {
            A.arrayLeftDivideEquals(S);
            errorCount = try_failure(errorCount,"arrayLeftDivideEquals conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("arrayLeftDivideEquals conformance check... ","");
          }
          A.arrayLeftDivideEquals(R);
          try 
          {
            check(A,O);
            try_success("arrayLeftDivideEquals... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"arrayLeftDivideEquals... ","(M.\\M != ones)");
          }
          A = R.copy();
          try 
          {
            A = A / S;
            errorCount = try_failure(errorCount,"arrayRightDivide conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e )
          {
            try_success("arrayRightDivide conformance check... ","");
          }
          C = A / R;
          try 
          {
            check(C,O);
            try_success("arrayRightDivide... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"arrayRightDivide... ","(M./M != ones)");
          }
          try 
          {
            A /= S;
            errorCount = try_failure(errorCount,"arrayRightDivideEquals conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("arrayRightDivideEquals conformance check... ","");
          }
          A /= R;
          try {
            check(A,O);
            try_success("arrayRightDivideEquals... ","");
          } catch ( Exception e ) {
            errorCount = try_failure(errorCount,"arrayRightDivideEquals... ","(M./M != ones)");
          }
          A = R.copy();
          B = Matrix.random(A.rowDimension,A.colDimension);
          try 
          {
            S = A.arrayTimes(S);
            errorCount = try_failure(errorCount,"arrayTimes conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("arrayTimes conformance check... ","");
          }
          C = A.arrayTimes(B);
          try 
          {
            check(C /= B,A);
            try_success("arrayTimes... ","");
          } 
          catch ( Exception e )
          {
            errorCount = try_failure(errorCount,"arrayTimes... ","(A = R, C = A.*B, but C./B != A)");
          }
          try
          {
            A.arrayTimesEquals(S);
            errorCount = try_failure(errorCount,"arrayTimesEquals conformance check... ","nonconformance not raised");
          } 
          catch ( ArgumentException e ) 
          {
            try_success("arrayTimesEquals conformance check... ","");
          }
          A.arrayTimesEquals(B);
          try 
          {
            check(A /= B,R);
            try_success("arrayTimesEquals... ","");
          } 
          catch ( Exception e ) 
          {
            errorCount = try_failure(errorCount,"arrayTimesEquals... ","(A = R, A = A.*B, but A./B != R)");
          }

    /*   
          I/O methods:
             read
             print
             serializable:
               writeObject
               readObject
    
          print("\nTesting I/O methods...\n");
             try {
                DecimalFormat fmt = new DecimalFormat("0.0000E00");
	        fmt.setDecimalFormatSymbols(new DecimalFormatSymbols(Locale.US));

                PrintWriter FILE = new PrintWriter(new FileOutputStream("JamaTestMatrix.out"));
                A.print(FILE,fmt,10);
                FILE.close();
                R = Matrix.read(new BufferedReader(new FileReader("JamaTestMatrix.out")));
                if (A.minus(R).norm1() < .001 ) {
                   try_success("print()/read()...","");
                } else {
                   errorCount = try_failure(errorCount,"print()/read()...","Matrix read from file does not match Matrix printed to file");
                }
             } catch ( java.io.IOException ioe ) {
               warningCount = try_warning(warningCount,"print()/read()...","unexpected I/O error, unable to run print/read test;  check write permission in current directory and retry");
             } catch(Exception e) {
                try {
                   e.printStackTrace(System.out);
                   warningCount = try_warning(warningCount,"print()/read()...","Formatting error... will try JDK1.1 reformulation...");
                   DecimalFormat fmt = new DecimalFormat("0.0000");
                   PrintWriter FILE = new PrintWriter(new FileOutputStream("JamaTestMatrix.out"));
                   A.print(FILE,fmt,10);
                   FILE.close();
                   R = Matrix.read(new BufferedReader(new FileReader("JamaTestMatrix.out")));
                   if (A.minus(R).norm1() < .001 ) {
                      try_success("print()/read()...","");
                   } else {
                      errorCount = try_failure(errorCount,"print()/read() (2nd attempt) ...","Matrix read from file does not match Matrix printed to file");
                   }
                } catch ( java.io.IOException ioe ) {
                  warningCount = try_warning(warningCount,"print()/read()...","unexpected I/O error, unable to run print/read test;  check write permission in current directory and retry");
             }
          }
           Stream IO Testting
          R = Matrix.random(A.rowDimension,A.colDimension);
          String tmpname = "TMPMATRIX.serial";
          try {
             ObjectOutputStream out = new ObjectOutputStream(new FileOutputStream(tmpname));
             out.writeObject(R);
             ObjectInputStream sin = new ObjectInputStream(new FileInputStream(tmpname));
             A = (Matrix) sin.readObject();
 
             try {
                check(A,R);
                try_success("writeObject(Matrix)/readObject(Matrix)...","");
             } catch ( Exception e ) {
               errorCount = try_failure(errorCount,"writeObject(Matrix)/readObject(Matrix)...","Matrix not serialized correctly");
             }
          } catch ( java.io.IOException ioe ) {
             warningCount = try_warning(warningCount,"writeObject()/readObject()...","unexpected I/O error, unable to run serialization test;  check write permission in current directory and retry");
          } catch(Exception e) {
             errorCount = try_failure(errorCount,"writeObject(Matrix)/readObject(Matrix)...","unexpected error in serialization test");
          }
           * /

    /**
          LA methods:
             transpose
             times
             cond
             rank
             det
             trace
             norm1
             norm2
             normF
             normInf
             solve
             solveTranspose
             inverse
             chol
             eig
             lu
             qr
             svd 
    **/

          print("\nTesting linear algebra methods...\n");
          A = new Matrix(columnwise,3);
          T = new Matrix(tvals);
          T = A.transpose();
          try 
          {
             check(A.transpose(),T);
             try_success("transpose...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"transpose()...","transpose unsuccessful");
          }
          A.transpose();
          try
          {
             check(A.norm1(),columnsummax);
             try_success("norm1...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"norm1()...","incorrect norm calculation");
          }
          try 
          {
             check(A.normInf(),rowsummax);
             try_success("normInf()...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"normInf()...","incorrect norm calculation");
          }
          try 
          {
             check(A.normF(),Math.Sqrt(sumofsquares));
             try_success("normF...","");
          } catch ( Exception e )
          {
             errorCount = try_failure(errorCount,"normF()...","incorrect norm calculation");
          }
          try 
          {
             check(A.trace(),sumofdiagonals);
             try_success("trace()...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"trace()...","incorrect trace calculation");
          }
          try 
          {
             check(A.getMatrix(0,A.rowDimension-1,0,A.rowDimension-1).det(),0.0);
             try_success("det()...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"det()...","incorrect determinant calculation");
          }
          SQ = new Matrix(square);
          try 
          {
             check(A * A.transpose(), SQ);
             try_success("times(Matrix)...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"times(Matrix)...","incorrect Matrix-Matrix product calculation");
          }
          try 
          {
             check(A * 0.0 ,Z);
             try_success("times(double)...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"times(double)...","incorrect Matrix-scalar product calculation");
          }

          A = new Matrix(columnwise,4);
          QRDecomposition QR = A.qr();
          R = QR.getR();
          try
          {
             check(A,QR.getQ().times(R));
             try_success("QRDecomposition...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"QRDecomposition...","incorrect QR decomposition calculation");
          }
          SingularValueDecomposition SVD = A.svd();
          try 
          {
             check(A,SVD.getU().times(SVD.getS().times(SVD.getV().transpose())));
             try_success("SingularValueDecomposition...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"SingularValueDecomposition...","incorrect singular value decomposition calculation");
          }
          DEF = new Matrix(rankdef);
          try 
          {
             check(DEF.rank(),Math.Min(DEF.rowDimension,DEF.colDimension)-1);
             try_success("rank()...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"rank()...","incorrect rank calculation");
          }
          B = new Matrix(condmat);
          SVD = B.svd(); 
          double [] singularvalues = SVD.getSingularValues();
          try 
          {
             check(B.cond(),singularvalues[0]/singularvalues[Math.Min(B.rowDimension,B.colDimension)-1]);
             try_success("cond()...","");
          } 
          catch ( Exception e )
          {
             errorCount = try_failure(errorCount,"cond()...","incorrect condition number calculation");
          }
          int n = A.colDimension;
          A = A.getMatrix(0,n-1,0,n-1);
          A[0,0] = 0.0;
          LUDecomposition LU = A.lu();
          try
          {
             check(A.getMatrix(LU.getPivot(),0,n-1),LU.getL().times(LU.getU()));
             try_success("LUDecomposition...","");
          } 
          catch ( Exception e )
          {
             errorCount = try_failure(errorCount,"LUDecomposition...","incorrect LU decomposition calculation");
          }
          X = A.inverse();
          try
          {
             check(A * X,Matrix.identity(3,3));
             try_success("inverse()...","");
          } 
          catch ( Exception e )
          {
             errorCount = try_failure(errorCount,"inverse()...","incorrect inverse calculation");
          }
          O = new Matrix(SUB.rowDimension,1,1.0);
          SOL = new Matrix(sqSolution);
          SQ = SUB.getMatrix(0,SUB.rowDimension-1,0,SUB.rowDimension-1);
          try 
          {
             check(SQ.solve(SOL),O); 
             try_success("solve()...","");
          } 
          catch ( ArgumentException e1 ) 
          {
             errorCount = try_failure(errorCount,"solve()...",e1.Message);
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"solve()...",e.Message);
          }
          A = new Matrix(pvals);
          CholeskyDecomposition Chol = A.chol(); 
          Matrix L = Chol.getL();
          try 
          {
             check(A,L.times(L.transpose()));
             try_success("CholeskyDecomposition...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"CholeskyDecomposition...","incorrect Cholesky decomposition calculation");
          }
          X = Chol.solve(Matrix.identity(3,3));
          try 
          {
             check(A*X, Matrix.identity(3,3));
             try_success("CholeskyDecomposition solve()...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"CholeskyDecomposition solve()...","incorrect Choleskydecomposition solve calculation");
          }
          EigenvalueDecomposition Eig = A.eig();
          Matrix D = Eig.getD();
          Matrix V = Eig.getV();
          try
          {
             check(A * V,V * D);
             try_success("EigenvalueDecomposition (symmetric)...","");
          } 
          catch ( Exception e )
          {
             errorCount = try_failure(errorCount,"EigenvalueDecomposition (symmetric)...","incorrect symmetric Eigenvalue decomposition calculation");
          }
          A = new Matrix(evals);
          Eig = A.eig();
          D = Eig.getD();
          V = Eig.getV();
          try 
          {
             check(A * V,V * D);
             try_success("EigenvalueDecomposition (nonsymmetric)...","");
          } 
          catch ( Exception e )
          {
             errorCount = try_failure(errorCount,"EigenvalueDecomposition (nonsymmetric)...","incorrect nonsymmetric Eigenvalue decomposition calculation");
          }

          try 
          {
	          print("\nTesting Eigenvalue; If this hangs, we've failed\n");
	          Matrix bA = new Matrix(badeigs);
	          EigenvalueDecomposition bEig = bA.eig();
	          try_success("EigenvalueDecomposition (hang)...","");
          } 
          catch ( Exception e ) 
          {
             errorCount = try_failure(errorCount,"EigenvalueDecomposition (hang)...",
				      "incorrect termination");
          }


          print("\nTestMatrix completed.\n");
          print("Total errors reported: " + errorCount.ToString() + "\n");
          print("Total warnings reported: " + warningCount.ToString() + "\n");

          Console.ReadLine();
       }

       /** private utility routines **/

       /** Check magnitude of difference of scalars. **/

       private static void check(double x, double y) 
       {
          double eps = Math.Pow(2.0,-52.0);
          if (x == 0 & Math.Abs(y) < 10*eps) return;
          if (y == 0 & Math.Abs(x) < 10*eps) return;
          if (Math.Abs(x-y) > 10*eps*Math.Max(Math.Abs(x),Math.Abs(y)))
          {
             throw new Exception("The difference x-y is too large: x = " + x.ToString() + "  y = " + y.ToString());
          }
       }

       /** Check norm of difference of "vectors". **/

       private static void check(double[] x, double[] y) 
       {
          if (x.Length == y.Length ) 
          {
             for (int i=0;i<x.Length;i++) 
             {
                check(x[i],y[i]);
             } 
          } 
          else
          {
             throw new Exception("Attempt to compare vectors of different lengths");
          }
       }

       /** Check norm of difference of arrays. **/

       private static void check(double[][] x, double[][] y) 
       {
          Matrix A = new Matrix(x);
          Matrix B = new Matrix(y);
          check(A,B);
       }

       /** Check norm of difference of Matrices. **/

       private static void check(Matrix X, Matrix Y) 
       {
          double eps = Math.Pow(2.0,-52.0);
          if (X.norm1() == 0.0 & Y.norm1() < 10*eps) return;
          if (Y.norm1() == 0.0 & X.norm1() < 10*eps) return;
          if ((X-Y).norm1() > 1000*eps*Math.Max(X.norm1(),Y.norm1())) 
          {
             throw new Exception("The norm of (X-Y) is too large: " +  (X - Y).norm1().ToString());
          }
       }

       /** Shorten spelling of print. **/

       private static void print (String s)
       {
          Console.Write(s);
       }

      /** Print appropriate messages for successful outcome try **/

       private static void try_success (String s,String e) 
       {
          Console.WriteLine(">    " + s + "success");
          if ( e != "" ) 
          {
            Console.WriteLine(">      Message: " + e );
          }
       }
      /** Print appropriate messages for unsuccessful outcome try **/

       private static int try_failure (int count, String s,String e) 
       {
          Console.WriteLine(">    " + s + "*** failure ***\n>      Message: " + e);
          return ++count;
       }

      /** Print appropriate messages for unsuccessful outcome try **/

       private static int try_warning (int count, String s,String e) 
       {
          Console.WriteLine(">    " + s + "*** warning ***\n>      Message: " + e );
          return ++count;
       }

       /** Print a row vector. **/

       private static void print(double[] x, int w, int d)
       {
          // Use format Fw.d for all elements.
          Console.WriteLine();
          Console.WriteLine(new Matrix(x,1).toFormatString(w,d));
          Console.WriteLine();
       }

    }

   
}
