This is the format for a cgil file (.gil file compiled for libraries). A cgil file is binary. 

Metadata
========
CGIL version - 1 byte (loc 0)
TargetLength - 1 byte (loc 1)
Target string (loc 2 - TargetLength + 1)
NumSequences - 2 bytes (loc TargetLength + 2 - TargetLength + 4)
NumOps - 2 bytes (loc TargetLength + 5 - TargetLength + 7)
[ CGIL version(0) TargetLength = 2(1) Target string(2-3) NumSequences(4-6) NumOps(7-9) ]


Sequence format
===============
NameLength - 1 byte (loc 0)
Name string (loc 1 - NameLength)
{
    EncodedDNA
    ----------
    DNALength - 3 bytes (loc NameLength + 1 - NameLength + 4)
    DNA string (loc NameLength + 1 - NameLength + 5 + DNALength)
}