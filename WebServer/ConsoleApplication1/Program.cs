using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace ConsoleApplication1
{
    internal class Program
    {
        private static Point[] horseSteps = new[]
        {
            new Point(1, -2),
            new Point(2, -1),
            new Point(-1, -2),
            new Point(2, 1),
            new Point(1, 2),
            new Point(-1, 2),
            new Point(-2, -1),
            new Point(-2, 1),
        };

        private static string foundedFrom="";

        public static void Main(string[] args)
        {
            
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5550);
            
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipPoint);
            server.Listen(100);
            Console.WriteLine("Server started");

            while (true)
            {
                Console.WriteLine("waiting..");
               
                Socket client = server.Accept();
                
                byte[] data = new byte[1024];
                int b = client.Receive(data);
               
                string movejson = Encoding.ASCII.GetString(data, 0, b);
                Console.WriteLine("i've got a json:"+movejson);
                
                try
                {
                    Move ob = JsonConvert.DeserializeObject<Move>(movejson);
                    
                    Point from = ParsePoint(ob.from);
                    Point to = ParsePoint(ob.to);
                    
                    string path = FindPath(from, to);
                    
                    Console.WriteLine("founded path="+path);
                    
                    client.Send(Encoding.ASCII.GetBytes("200 OK "+path));
                    
                }
                catch (JsonReaderException e)
                {
                    
                    client.Send(Encoding.ASCII.GetBytes("404 NO"));
                    Console.WriteLine(e);
                }
            }
        }

        private static string FindPath(Point from, Point to)
        {
            string path = to.ToString()+"\"]";
            while(!from.Equals(to))
            {
            List<Point> currentwave = new List<Point> {from};

            while (!CheckTo(currentwave, to))
            {
                List<Point> nextvawe = new List<Point>();
                foreach (Point nextpoint in currentwave)
                {

                    foreach (Point offset in horseSteps)
                    {
                        Point p = ShiftPoint(nextpoint, offset);
                        
                        p.from = nextpoint.ToString();
                        /*
                         * здесь можно вставить проверку не занято ли поле с координатами р
                         *
                         */
                        nextvawe.Add(p);
                    }

                    currentwave = nextvawe;
                    
                }
                
            }
                path ="\""+ foundedFrom+"\"," + path;

            to = ParsePoint(foundedFrom);
        }
            
            return "["+path;

        }

        private static bool CheckTo(List<Point> vawe, Point to)
        {
            foreach (Point p in vawe)
            {
                if (p.x == to.x && p.y == to.y)
                {
                    foundedFrom=p.from;
                    return true;
                }

            }

            return false;

        }

        private static Point ShiftPoint(Point start, Point offset)
        {
            Point p = new Point(start.x + offset.x, start.y + offset.y);
            return p;
        }

        private static Point ParsePoint(string pos)
        {
            const string columns = "abcdefgh";
            
            Point p = new Point();
            p.x=columns.IndexOf(pos[0]);
            p.y = int.Parse(pos[1].ToString());
            
            return p;

        }
    }

    internal class Move
    {
        public string from="";
        public string to="";
    }

    internal struct Point
    {
        public int x;
        public int y;
        public string from;
        
        public bool Equals(Point obj)
        {
            return (x == obj.x) && (y == obj.y);
        }

        public override string ToString()
        {
            const string columns = "abcdefgh";
            if (x < 8)
                return columns[x] + y.ToString();
            else return "ZZ";
        }

        public Point(int xx, int yy)
        {
            x = xx;
            y = yy;
            from = "";
        }


        
    }
}