REF 'IO', 'Math' FROM 'Runtime'

{
	Tests Runtime.Math functions.
	Checks if sqrt(4) = abs(-2).
	If so, will type "Success".
}

? Check if sqrt(4) = abs(-2) (Hint: it does)
If Sqrt(4) = Abs(-2)
[
	? Types "Success!"
	Type("Success!")
]
[
	Type("FAILED")
]