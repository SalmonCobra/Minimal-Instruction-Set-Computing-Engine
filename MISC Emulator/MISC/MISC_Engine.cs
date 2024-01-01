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
            opcode = binaryRam.Substring(0, 4);
            regDest = binaryRam.Substring(5, 9);
            regSrc = binaryRam.Substring(6, 14);
            data = binaryRam.Substring(14, 47);
        }
        public static void ReadRam(bool resetVars)
        {
            if (resetVars) { opcode = ""; regDest = ""; regSrc = ""; data = ""; }
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
