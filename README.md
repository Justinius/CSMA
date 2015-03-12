# CSMA
Port of JAMA to C#

This is a port of the NIST JAMA library to C#.
I have kept things as similar to the original usage as possible. 

I added operator overloading library so you can write: C = A*B instead of C = A.time(B)
I've also added getters for the matrix dimensions instead of having to call getRowDimentions etc.

This is not an optimized matrix library. There is no calls to native helpers. It is a straight forward implemtation
of matrix routines in C#. 

However, if a user wishes to exploit the TPL or SIMD routines to improve performance that would be great.

The original JAMA code included the following:
Copyright Notice This software is a cooperative product of The MathWorks and the 
National Institute of Standards and Technology (NIST) which has been released to the public domain. 
Neither The MathWorks nor NIST assumes any responsibility whatsoever for its use by other parties, 
and makes no guarantees, expressed or implied, about its quality, reliability, or any other characteristic.

As such I'm pretty much going to leave the project in the public domain as well.
