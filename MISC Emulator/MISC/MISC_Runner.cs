using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MISC
{
    internal class MISC_Runner
    {
        //config variables

        public static string[] ram = { };
        public static int bits = 0;
        public static int rows = 0;
        public static int confRegisters = 0;
        public static double regAddressLength = 0d;
        public static int[] display = new int[2];
        public static bool configStatus = false;
        public static bool debug = false;

        public static string[] registers = { };

        static void Main(string[] args)
        {
            Configerator();
            WriteRom();
            Console.WriteLine("Query: Run Program.bin?");
            Console.ReadLine();
            Console.Clear();

        }

        // Configue the emulator from the config file.
        public static void Configerator()
        {
            int opcodeLength = 5;
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
            // Configure Registers.
            Console.WriteLine("Info: Instanciating Registers.");
            registers = new string[confRegisters + 4];
            for (int i = 0; i < registers.Length; i++)
            {
                for (int j = 0; j < bits + opcodeLength + regAddressLength * 2; j++)
                {
                    registers[i] += "0";
                }
                if (debug) { Console.WriteLine("Debug: " + registers[i].ToString()); }
            }
            Console.WriteLine("Info: Finished Instanciating Registers!");


            // Configure and Instanciate Ram.

            if (configStatus)
            {
                Console.WriteLine("Info: Instanciating RAM.");
                ram = new string[rows + (display[0] * display[1]) / bits];
                for (int i = 0; i < ram.Length; i++)
                {
                    string nopdata = "";
                    for (int j = 0; j < bits + opcodeLength + regAddressLength * 2; j++) // FIX: need to account for register count too!
                    {
                        nopdata += "0";
                    }
                    ram[i] = nopdata;
                    if (debug) { Console.WriteLine("Debug: " + ram[i].ToString()); }
                    if (i == ram.Length - 1)
                    {
                        Console.WriteLine("Info: Finished Instanciating RAM!");
                    }
                    
                }
            }


            // Display completion status of configurator to console.
            if (configStatus)
            {
                Console.WriteLine("Info: Configuration Complete");
            }
            else
            {
                Console.WriteLine("Error: Configuration Failed! Read log for more details");
            }
        }

        // Write the binary file to the ram.
        private static void WriteRom()
        {
            if (configStatus)
            {
                string[] binary = System.IO.File.ReadAllLines("C:\\Users\\Benjamin\\Desktop\\MISC\\Minimal-Instruction-Set-Emulator\\MISC Emulator\\MISC\\Program.bin");
                int writeRamIndex = 0;
                Console.WriteLine("Info: Writing ROM to RAM Started");
                if (debug) { Console.WriteLine("Binaries:"); }
                foreach (string line in binary)
                {
                    string code = "";
                    if (line.Contains(" ") || line.Contains("#"))
                    {
                        code = line.Substring(0, line.IndexOf("#"));
                    }
                    else
                    {
                        code = line;
                    }
                    if (code.Contains("-") || code.Contains("0b") || code.Contains(" "))
                    {
                        code = code.Replace("-", "").Replace("0b", "").Replace(" ", "");
                    }
                    ram[writeRamIndex] = code;
                    if (debug) { Console.WriteLine("Debug: " + code); }
                    writeRamIndex++;
                }
                Console.WriteLine("Info: Finished Writing to RAM");
            }

        }

    }
}
