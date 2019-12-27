using System;
using System.Configuration;

namespace ERDA
{
   

    public class HandleConfiguration
    {

        public static void EncriptarConfiguracion(string exeConfigName)
        {
            // Takes the executable file name without the
            // .config extension.
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.OpenExeConfiguration(exeConfigName);

                ConnectionStringsSection section = (ConnectionStringsSection)config.GetSection("connectionStrings");

                if (!section.SectionInformation.IsProtected)
                {
                    // Encrypt the section.
                    section.SectionInformation.ProtectSection("RSAProtectedConfigurationProvider");
                }

                section.SectionInformation.ForceSave = true;

                // Save the current configuration.
                config.Save();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static System.Configuration.Configuration DesEncriptarConfiguracion(string exeConfigName)
        {
            // Takes the executable file name without the
            // .config extension.
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.OpenExeConfiguration(exeConfigName);

                ConnectionStringsSection section = (ConnectionStringsSection)config.GetSection("connectionStrings");

                if (section.SectionInformation.IsProtected)
                {
                    // Remove encryption.
                    section.SectionInformation.UnprotectSection();
                }

                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }



    }

}
