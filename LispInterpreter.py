
"""
Sam Perry and Erik Buinevicius

Our project is a Lisp Interpreter, based on Kamin's Lisp in Pascal, written in python.

It will read text files in Lisp Style and execute certain commands:

    WHILE               CONS
    SET                 CAR
    BEGIN               CDR
    +                   NUMBER?
    -                   SYMBOL?
    *                   LIST?
    /                   NULL?
    =                   PRINT
    <                   T
    >                   IF

"""


"""

    If an "IF" is read in the text.

"""
def IfState():
    print ("IF STATEMENT READ")


"""

    If a "WHILE" is read in the text.

"""
def WhileState():
    print ("WHILE STATEMENT READ")


"""

    If a "BEGIN" is read in the text.

"""
def BeginState():
    print ("BEGIN STATEMENT READ")


"""

    If a "SET" is read in the text.

"""
def SetState():
    print ("SET STATEMENT READ")





