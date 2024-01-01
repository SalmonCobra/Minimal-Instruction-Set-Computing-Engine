namespace MISC
{
    internal class MISCRUNNER
    {
        public static string[] ram = { "" };
        public static int bits = 0;
        public static int rows = 0;
        public static int registers = 0;
        public static double regAddressLength = 0d;
        public static int[] display = { 0, 0 };

        public static bool configStatus = false;
        //public static FileStream logfile = File.Create(".");

        static void Main(string[] args)
        {
            ReadConfig();
            InstanciateRam();
            Console.WriteLine("Run Program.bin?");
            Console.ReadLine();

        }

        // Configue the emulator from the config file.
        public static void ReadConfig()
        {
            configStatus = true;
            Console.WriteLine("Configuring Emulator.");
            string[] config = System.IO.File.ReadAllLines("C:\\Users\\Benjamin\\Desktop\\MISC\\Minimal-Instruction-Set-Emulator\\MISC Emulator\\MISC\\config.conf");
            foreach (string line in config)
            {
                string newLine = line.Replace(" ", "");
                if (newLine.Contains("=") && newLine.Contains(";"))
                {
                    switch (newLine.Substring(0, line.IndexOf("=") - 1))
                    {
                        case "Bits":
                            bits = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            if (bits < 4)
                            {
                                Console.WriteLine("Config Error: Value of \"Bits\" is either Null or less than 4");
                                configStatus = false;
                            }
                            break;
                        case "Rows":
                            rows = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            if (rows == 0)
                            {
                                Console.WriteLine("Config Error: Value of \"Rows\" is Null or Zero");
                                configStatus = false;
                            }
                            break;
                        case "Registers":
                            registers = int.Parse(newLine.Substring(newLine.IndexOf("=") + 1, (newLine.IndexOf(";")) - (newLine.IndexOf("=")) - 1));
                            for (int i = 0; Math.Pow(2, i) < registers + 2; i++)
                            {
                                regAddressLength++;
                            }
                            Console.WriteLine("Register Address Length: " + regAddressLength.ToString() + " Bits.");
                            if (registers < 2)
                            {
                                Console.WriteLine("Config Error: Value of \"Registers\" is Out of Range");
                                configStatus = false;
                            }
                            break;
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

                    }
                }
            }
            if (configStatus)
            {
                Console.WriteLine("Configuration Complete");
            }
            else
            {
                Console.WriteLine("Configuration Failed! Read log for more details");
            }

        }
        // Set all bits in ram to 0.
        public static void InstanciateRam()
        {
            if (configStatus)
            {
                Console.WriteLine("Instanciating RAM.");
                ram = new string[rows + (display[0] * display[1]) / bits];
                for (int i = 0; i < ram.Length; i++)
                {
                    string nopdata = "";
                    for (int j = 0; j < bits; j++) // FIX: need to account for register count too!
                    {
                        nopdata += "0";
                    }
                    ram[i] = nopdata;
                    if (i == ram.Length - 1)
                    {
                        Console.WriteLine("Finished!");
                    }
                }
            }

        }
        // Write the binary file to the ram.
        private static void WriteRom()
        {
            if (configStatus)
            {
                string[] binary = System.IO.File.ReadAllLines("C:\\Users\\Benjamin\\Desktop\\MISC\\Minimal-Instruction-Set-Emulator\\MISC Emulator\\MISC\\Program.bin");
                int writeRamIndex = 0;
                foreach (string line in binary)
                {
                    Console.WriteLine("Writing ROM to RAM Started");

                    string code = "";
                    if (line.Contains(" ") || line.Contains("#"))
                    {
                        code = (((line.Substring(0, line.IndexOf("#"))).Replace("-", "")).Replace("0b", "")).Replace(" ", "");
                    }
                    else
                    {
                        code = line;
                    }
                    ram[writeRamIndex] = code;
                    writeRamIndex++;
                }
            }

        }

    }
}
