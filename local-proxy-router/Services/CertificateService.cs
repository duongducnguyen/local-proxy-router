using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace LocalProxyRouter.Services
{
    public class CertificateService
    {
        public static void CreateRootCertificate()
        {
            // Path to the cert directory and cert.pfx file
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "cert");
            string filePath = Path.Combine(directory, "cert.pfx");

            // Check if the certificate file already exists
            if (File.Exists(filePath))
            {
                Console.WriteLine($"Certificate already exists at {filePath}. No action taken.");
                return; // If it exists, do nothing
            }

            // Check and create the directory if it doesn't exist
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create a new RSA key
            using (var rsa = RSA.Create(2048))
            {
                // Create a certificate request
                var request = new CertificateRequest("cn=LocalProxyRouter", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var cert = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

                // Export the certificate to a file
                var certBytes = cert.Export(X509ContentType.Pfx, "duongducnguyen");
                File.WriteAllBytes(filePath, certBytes);

                Console.WriteLine($"Root certificate created and exported to {filePath}");
            }
        }
    }
}