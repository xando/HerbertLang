# Setup

* Install CLI `dotnet` 
* Restore Project `dotnet restore`

# Build 

`dotnet build`

# Test

`dotnet test`

# Notes 

```
-> Solution 
   - HerbertLang (There is a reference set to TEST dotnet add ... reference)
   - HerbertLang.Tests 
```

# Flow 

```
LEXER -> tokens
PARSER -> ast
INTEPRETER -> code/string -> code/steps 
```

# Idea 

```python

   # simple 
   sss

   # function no args
   f:sss
   f

   f():sss
   f()

   # function step args
   f(A):A
   f(sss)

   f(B):aBa
   f(sss)

   # function deep args
   f:f=@
   f=0

```