using System.ComponentModel.Design;

namespace MISC
{
    internal class MISC_Engine
    {

        #region Public Variables
        //config variables

        public static long[] ram = Array.Empty<long>();
        public static long[] registers = Array.Empty<long>();

        public static long[] stack = Array.Empty<long>();
        public static int stackAddress = 0;

        public static int[] display = new int[2];

        public static int bits = 0;
        public static int rows = 0;
        public static int confRegisters = 0;
        public static double regAddressLength = 0d;

        public static bool configStatus = false;
        public static bool debug = false;

        public static Dictionary<int, string> Opcodes = new Dictionary<int, string>()
        {
            { 0, "nop" },
            { 1, "mov" },
            { 10, "push" },
            { 11, "pop" },
            { 100, "add" },
            { 101, "sub" },
            { 110, "and" },
            { 111, "or" },
            { 1000, "xor" },
            { 1001, "not" },
            { 1010, "als" },
            { 1011, "ars" },
            { 1100, "in" },
            { 1101, "out" },
            { 1110, "cmp" },
            { 1111, "jmp" },
            { 10000, "jlt" },
            { 10001, "jgt" },
            { 10010, "je" },
            { 10011, "ret" },
            { 10100, "halt" }
        };

        #endregion
        public enum Register : int
        {
            PC,
            None,
            EAX,
            EBX
        }
        static void Main(string[] args)
        {
            Configerator();
            DebugRam(0, true);
        }
        #region Methods
        // Configue the emulator from the config file.
        private static void Configerator()
        {
            #region Lexer
            int additional_registers = 4;
            configStatus = true;
            Console.WriteLine("Info: Configuring Emulator.");
            string[] config = System.IO.File.ReadAllLines("C:\\Users\\Benjamin\\Desktop\\MISC\\Minimal-Instruction-Set-Emulator\\MISC Emulator\\MISC\\config.conf");
            foreach (string line in config)
            {
                string newLine = line.Replace(" ", "");
                if (newLine.Contains("=") && newLine.Contains(";"))
                {
                    switch (newLine.Substring(0, line.IndexOf("=") - 1))
                    {
                        // How many bits.
                        case "Bits":
                            bits = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            if (bits < 4)
                            {
                                Console.WriteLine("Config Error: Value of \"Bits\" is either Null or less than 4");
                                configStatus = false;
                            }
                            break;
                        // How many rows.
                        case "Rows":
                            rows = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            if (rows == 0)
                            {
                                Console.WriteLine("Config Error: Value of \"Rows\" is Null or Zero");
                                configStatus = false;
                            }
                            break;
                        // How many Registers.
                        case "Registers":
                            confRegisters = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            for (int i = 0; Math.Pow(2, i) < confRegisters + 4; i++)
                            {
                                regAddressLength++;
                            }
                            Console.WriteLine("Info: Register Address Length: " + regAddressLength.ToString() + " Bits.");
                            if (confRegisters < 2)
                            {
                                Console.WriteLine("Config Error: Value of \"Registers\" is Out of Range");
                                configStatus = false;
                            }
                            break;
                        // Size of the Display.
                        case "Display_size":
                            if (newLine.Contains("x"))
                            {
                                display[0] = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf("x") - newLine.IndexOf("=")) - 1));
                                display[1] = int.Parse(newLine.Substring(newLine.IndexOf("x") + 1, (newLine.IndexOf(";") - newLine.IndexOf("x")) - 1));
                                if (display[0] < 1 || display[1] < 1)
                                {
                                    Console.WriteLine("Config Error: Value of \"Display_size\" Out of Range");
                                    configStatus = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Config Error: \"Display_size\" missing \"**x**\".");
                                configStatus = false;
                            }
                            break;
                        case "Debug":
                            debug = bool.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            if (debug) { Console.WriteLine("Info: Debug Enabled"); } else { Console.WriteLine("Info: Debug Disabled"); }
                            break;

                    }
                }
            }
            #endregion

            #region Initilizers
            if (configStatus)
            {
                // Configure and Initializing Registers.
                Console.WriteLine("Info: Initializing Registers.");
                registers = new long[confRegisters + additional_registers];
                for (int i = 0; i < registers.Length; i++)
                {
                    registers[i] = 0;
                    if (debug) { Console.WriteLine("Debug: " + registers[i].ToString()); }
                }
                Console.WriteLine("Info: Finished Initializing Registers!");

                
                // Configure and Initialize Stack
                Console.WriteLine("Info: Initializing Stack.");
                stack = new long[64000]; // 2MB when set to 32 bit
                for (int i = 0;i < stack.Length;i++)
                {
                    stack[i] = 0;
                    if (debug) { Console.WriteLine("Debug: " + stack[i].ToString()); }
                }
                Console.WriteLine("Info: Finished Initializing Stack");

                // Configure and Initializing Ram.

                Console.WriteLine("Info: Initializing RAM.");
                ram = new long[rows + (display[0] * display[1]) / bits];
                for (int i = 0; i < ram.Length; i++)
                {
                    ram[i] = 0;
                    if (debug) { Console.WriteLine("Debug: " + ram[i].ToString()); }                   
                }
                Console.WriteLine("Info: Finished Initializing RAM!");

                Console.WriteLine("Info: Configuration Complete");
            } else
            {
                Console.WriteLine("Error: Configuration Failed! Read log for more details");
            }

            #endregion

            #region Load Operations
            // Load Rom
            if (configStatus)
            {
                string[] binary = System.IO.File.ReadAllLines("C:\\Users\\Benjamin\\Desktop\\MISC\\Minimal-Instruction-Set-Emulator\\MISC Emulator\\MISC\\Program.bin");
                int writeRamIndex = 0;
                Console.WriteLine("Info: Loading ROM");
                if (debug) { Console.WriteLine("Binaries:"); }
                foreach (string line in binary)
                {
                    string precode = "";
                    long code = 0b0;
                    if (line.Contains(" ") || line.Contains("#"))
                    {
                        precode = line.Substring(0, line.IndexOf("#"));
                    }
                    else
                    {
                        code = Convert.ToInt64(line.Replace("-", "").Replace("0b", "").Replace(" ", ""), 2);
                    }
                    if (precode.Contains("-") || precode.Contains("0b") || precode.Contains(" "))
                    {
                        code = Convert.ToInt64(precode.Replace("-", "").Replace("0b", "").Replace(" ", ""), 2);
                    }

                    ram[writeRamIndex] = code;
                    if (debug) { Console.WriteLine("Debug: " + code); }
                    writeRamIndex++;
                }
                Console.WriteLine("Info: Finished Loading ROM");
            }
            #endregion
        }

        #region RAM Methods

        public static string opcode = String.Empty;
        public static string regDest = String.Empty;
        public static string regSrc = String.Empty;
        public static string data = String.Empty;
        public static void ReadRam()
        {
            string binaryRam = Convert.ToString(ram[registers[(int)Register.PC]], 2);
            opcode = binaryRam.Substring(0, 5);
            regDest = binaryRam.Substring(5, 5);
            regSrc = binaryRam.Substring(10, 5);
            data = binaryRam.Substring(15, 32);
        }
        public static void ReadRam(bool resetVars)
        {
            if (resetVars) { opcode = ""; regDest = ""; regSrc = ""; data = ""; }
        }
        public static void DebugRam(int address)
        {
            Console.WriteLine(Convert.ToString(ram[address], 2));
        }
        public static void DebugRam(int address, bool H)
        {
            if (H)
            {
                string PC = Convert.ToString(registers[0], 2);
                WriteReg(registers[(int)Register.PC].ToString(), address);
                ReadRam();
                if (Opcodes.ContainsKey(Convert.ToInt32(opcode)))
                {
                    Console.Write(Opcodes[Convert.ToInt32(opcode)] + "-");
                } else
                {
                    Console.WriteLine("Error: Not an Opcode");
                    Console.Write(">" + opcode + "< ");
                }
                var containsDefaultReg = false;
                switch (regDest)
                {
                    case "00000":
                        Console.Write("Program_Counter-");
                        containsDefaultReg = true;
                    break;
                    case "00001":
                        Console.Write("Null-");
                        containsDefaultReg = true;
                        break;
                    case "00010":
                        Console.Write("EAX-");
                        containsDefaultReg = true;
                        break;
                    case "00011":
                        Console.Write("EBX-");
                        containsDefaultReg = true;
                        break;
                }
                if (!containsDefaultReg) { Console.Write(Convert.ToInt64(regDest, 2) - 2 + "-"); }

                containsDefaultReg = false;
                switch (regSrc)
                {
                    case "00000":
                        Console.Write("Program_Counter-");
                        containsDefaultReg = true;
                        break;
                    case "00001":
                        Console.Write("Null-");
                        containsDefaultReg = true;
                        break;
                    case "00010":
                        Console.Write("EAX-");
                        containsDefaultReg = true;
                        break;
                    case "00011":
                        Console.Write("EBX-");
                        containsDefaultReg = true;
                        break;
                }
                if (!containsDefaultReg) { Console.Write(Convert.ToInt64(regSrc, 2) - 2 + "-"); }
                Console.Write($"{data}     Base10: {Convert.ToInt64(data, 2)}");
                ReadRam(true);
                WriteReg(registers[(int)Register.PC].ToString(), Convert.ToInt64(PC));
            }
            else
            {
                Console.WriteLine(Convert.ToString(ram[address], 2));
            }

        }

        #endregion

        #region Register Methods
        public static long ReadReg(string register)
        {
            return Convert.ToInt64(register);
        }
        public static void WriteReg(string register, long data)
        {
            registers[Convert.ToInt32(register)] = data;
        }

        #endregion

        #endregion
    }
}