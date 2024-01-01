# MISC
MISC (Minimal Instruction Set Computing) is a simple project simply for the enjoyment of computer science! Its a light CPU Emulator of my own architecture "very" lightly based on RISC. It is in very ealy stages of development so bare with me as I am only a Beginner/Intermediate programmer :).

I hope you enjoy this project and feel free to collaberate and share your creations!

Link to Discord.
https://discord.gg/zyMaFYkB
# Instructions
| Opcode | Discription | Binary Opcode |
| :---: | :--- | :---: |
| nop | No operation. | 0b0 |
| mov | Move. | 0b1 |
| push | Push to the stack. | 0b10 |
| pop | Pop the stack. | 0b11 |
| add | Add. | 0b100 |
| sub | Subtract. | 0b101 |
| and | Binary and. | 0b110 |
| or | Binary or. | 0b111 |
| xor | Binary xor. | 0b1000 |
| not | Binary not. | 0b1001 |
| als | Arithmetic left shift. | 0b1010 |
| ars | Arithmetic right shift. | 0b1011 |
| in  | Input from devices. | 0b1100 |
| out | Output to devices. | 0b1101 |
| cmp | Compare two registers. | 0b1110 |
| jmp | Jump to address. | 0b1111 |
| jlt | Jump if less than.  | 0b10000 |
| jgt | Jump if greater than. | 0b10001 |
| je | Jump if equal. | 0b10010 |
| halt | Halts the program. | 0b10011 |


## Registers
| Register | Discription | Binary Code |
| :---: | :--- | :---: |
| Null | No register. | 0b0 |
| eax | Accumulator register A. | 0b1 |
| ebx | Accumulator register B. | 0b10 |

## Notes

The amount of registers can be configured. Refer to config.conf.

Register "eax" is where results are stored.

Comment (#): Ignores everything after "#" on that line.
