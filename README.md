# DISH Mealy Machine: C-Style Comments

C# implementation using the letters of **DISH** as ordinary input symbols.
The comment structure and states come from [`../NFA/README.md`](../NFA/README.md)
and [`../NFA/program.cs`](../NFA/program.cs); each DISH letter takes the moves
originally assigned to `a`. The attached examples inform the output tape and trace.

## Definition

`M = (Q, Sigma, Gamma, delta, lambda, q0)`

- States: `Q = {q0, q1, q2, q3, q4, qD}`; start state: `q0`.
- Input alphabet: `Sigma = {d, i, s, h, *, /}`.
- Output alphabet: `Gamma = {0, 1}`.
- `delta` gives the next state; `lambda` gives the output of that move.
- Output `1` only when `q3` reads `/`, completing the first closing `*/`.
- The **final output must be `1`** for the whole input to be accepted.
  Mealy machines produce outputs rather than requiring a set of accepting states.

Letters can occur in any order and may repeat; the body need not spell `dish`.
Only lowercase `d`, `i`, `s`, `h`, `*`, `/` are allowed. Other characters,
including uppercase letters, spaces, and newlines, send the simulator to `qD`
with output `0`, permanently rejecting the input.

## Transition / output table

Each cell is `next state / output`.

| State | `d` | `i` | `s` | `h` | `*` | `/` |
| --- | --- | --- | --- | --- | --- | --- |
| `q0` (start) | qD / 0 | qD / 0 | qD / 0 | qD / 0 | qD / 0 | q1 / 0 |
| `q1` (opening slash) | qD / 0 | qD / 0 | qD / 0 | qD / 0 | q2 / 0 | qD / 0 |
| `q2` (body) | q2 / 0 | q2 / 0 | q2 / 0 | q2 / 0 | q3 / 0 | q2 / 0 |
| `q3` (star in body) | q2 / 0 | q2 / 0 | q2 / 0 | q2 / 0 | q3 / 0 | q4 / 1 |
| `q4` (completed) | qD / 0 | qD / 0 | qD / 0 | qD / 0 | qD / 0 | qD / 0 |
| `qD` (dead) | qD / 0 | qD / 0 | qD / 0 | qD / 0 | qD / 0 | qD / 0 |

The source NFA's empty-set moves become moves to `qD`. Keeping `q4` separate
preserves the source state meanings; this is not a minimality claim.
Every input character generates one output bit, even after an invalid move.

## Run

Requires the .NET 10 SDK. From `github_act_1`:

```powershell
dotnet run --project "Mealy Machine/MealyMachine.csproj"
```

Enter strings interactively; type `exit` to stop. Empty input is rejected.
Input is not trimmed or converted to lowercase.

Run the built-in tests (no extra packages):

```powershell
dotnet run --project "Mealy Machine/MealyMachine.csproj" -- --test
```

## Examples

| Input | Output | Result |
| --- | --- | --- |
| `/*dish*/` | `00000001` | ACCEPTED |
| `/*dis*h*/` | `000000001` | ACCEPTED |
| `/*dish/dish*/` | `0000000000001` | ACCEPTED |
| `/**/` | `0001` | ACCEPTED |
| `/*dish` | `000000` | REJECTED |
| `dish*/` | `000000` | REJECTED |
| `/*dish*/h` | `000000010` | REJECTED |
| `/*a*/` | `00000` | REJECTED (outside alphabet) |

For `/*dish*/`, the state trace is:

```text
q0 -> q1 -> q2 -> q2 -> q2 -> q2 -> q2 -> q3 -> q4
```

Only one entire block comment is recognized, not embedded comments or `//` line
comments. The first `*/` closes the comment. Any following character invalidates
the whole input, even though an earlier output bit was `1`.
