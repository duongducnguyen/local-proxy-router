using System;
using System.Threading.Tasks;
using LocalProxyRouter.Services;

namespace LocalProxyRouter
{
    internal class Program
    {
        private static ProxyService proxyService;


        static async Task Main(string[] args)
        {

            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            proxyService = new ProxyService();

            while (true)
            {
                Console.WriteLine("Chọn chức năng:");
                Console.WriteLine("1. Xuất file CRT");
                Console.WriteLine("2. Khởi động Proxy Server");
                Console.WriteLine("3. Thoát");
                Console.Write("Nhập lựa chọn của bạn (1-3): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CertificateService.CreateRootCertificate(); // Gọi phương thức xuất chứng chỉ
                        break;
                    case "2":
                        // Nhập IP và PORT từ người dùng
                        Console.Write("Nhập địa chỉ IP: ");
                        string ip = Console.ReadLine();

                        Console.Write("Nhập cổng: ");
                        if (int.TryParse(Console.ReadLine(), out int port))
                        {
                            await proxyService.StartProxyServer(ip, port);
                        }
                        else
                        {
                            Console.WriteLine("Cổng không hợp lệ. Vui lòng nhập lại.");
                        }
                        break;
                    case "3":
                        Console.WriteLine("Đang thoát...");
                        return; // Thoát khỏi ứng dụng
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng nhập lại.");
                        break;
                }

                Console.WriteLine(); // Thêm dòng trống để dễ đọc
            }
        }
    }
}