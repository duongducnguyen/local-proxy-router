using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace LocalProxyRouter.Services
{
    public class ProxyService
    {
        private ProxyServer proxyServer;
        private bool isRunning = true;
        private const string CertPassword = "duongducnguyen";
        private const string CertFolderName = "cert";
        private const string CertFileName = "cert.pfx";

        public async Task StartProxyServer(string ip, int port)
        {
            string certFolderPath = Path.Combine(Directory.GetCurrentDirectory(), CertFolderName);
            string certFilePath = Path.Combine(certFolderPath, CertFileName);

            try
            {
                // Khởi tạo thư mục chứng chỉ nếu chưa tồn tại
                if (!Directory.Exists(certFolderPath))
                {
                    Directory.CreateDirectory(certFolderPath);
                }

                // Khởi tạo proxy server
                proxyServer = new ProxyServer
                {
                    ForwardToUpstreamGateway = true,
                    EnableConnectionPool = true,
                    TcpTimeWaitSeconds = 30
                };

                proxyServer.CertificateManager.LoadRootCertificate(certFilePath, CertPassword, true);

                // Kiểm tra xem chứng chỉ đã được tin tưởng chưa
                if (!proxyServer.CertificateManager.IsRootCertificateMachineTrusted())
                {
                    Console.WriteLine("Installing root certificate...");
                    proxyServer.CertificateManager.TrustRootCertificate();
                }
                else
                {
                    Console.WriteLine("Root certificate is already trusted.");
                }

                // Cấu hình endpoint
                var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Parse(ip), port, true);


                // Thêm endpoint vào proxy server
                proxyServer.AddEndPoint(explicitEndPoint);

                // Cấu hình xử lý lỗi
                proxyServer.ExceptionFunc = exception =>
                {
                    Console.WriteLine($"[{DateTime.Now}] Proxy Error: {exception.Message}");
                    Console.WriteLine($"Stack trace: {exception.StackTrace}");
                };

                // Đăng ký các event handler
                proxyServer.BeforeRequest += OnRequest;
                proxyServer.BeforeResponse += OnResponse;

                // Bắt đầu proxy server
                proxyServer.Start();

                // In thông tin khi khởi động thành công
                Console.WriteLine($"[{DateTime.Now}] Proxy server running on {explicitEndPoint.IpAddress}:{explicitEndPoint.Port}");
                Console.WriteLine($"Root Certificate Path: {certFilePath}");
                Console.WriteLine("Press 'Q' to stop the server...");

                // Vòng lặp chờ lệnh dừng
                while (isRunning)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        isRunning = false;
                    }
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Critical Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                StopProxyServer();
            }
        }

        private async Task OnBeforeSslAuthenticate(object sender, BeforeSslAuthenticateEventArgs e)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now}] SSL Authentication for: {e.SniHostName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] SSL Authentication Error: {ex.Message}");
            }
        }

        public void StopProxyServer()
        {
            try
            {
                if (proxyServer != null)
                {
                    proxyServer.BeforeRequest -= OnRequest;
                    proxyServer.BeforeResponse -= OnResponse;
                    proxyServer.Stop();
                    Console.WriteLine($"[{DateTime.Now}] Proxy server stopped.");
                    proxyServer.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Error stopping proxy: {ex.Message}");
            }
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            try
            {
                string url = e.HttpClient.Request.Url;
                string method = e.HttpClient.Request.Method.ToUpper();
                Console.WriteLine($"[{DateTime.Now}] Request: {method} {url}");

                // Log headers for debugging
                foreach (var header in e.HttpClient.Request.Headers)
                {
                    Console.WriteLine($"Header: {header.Name}: {header.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Error processing request: {ex.Message}");
            }
        }

        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            try
            {
                if (e.HttpClient.Response != null)
                {
                    Console.WriteLine($"[{DateTime.Now}] Response for: {e.HttpClient.Request.Url}");
                    Console.WriteLine($"Status: {e.HttpClient.Response.StatusCode} {e.HttpClient.Response.StatusDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Error processing response: {ex.Message}");
            }
        }
    }
}