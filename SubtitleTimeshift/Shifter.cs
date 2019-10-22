using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SubtitleTimeshift
{
    public class Shifter
    {
        async static public Task Shift(Stream input, Stream output, TimeSpan timeSpan, Encoding encoding, int bufferSize = 1024, bool leaveOpen = false)
        {

            try
            {

                //Expressão Regular para pegar os blocos e separar em grupos
                var rgx = new Regex(@"(?<sequence>\d+)\r\n(?<start>\d{2}\:\d{2}\:\d{2},\d{3}) --\> (?<end>\d{2}\:\d{2}\:\d{2},\d{3})\r\n(?<text>[\s\S]*?\r\n\r\n)");

                //Parametros para ler o arquivo de entrada
                byte[] bytes = new byte[input.Length];
                int numBytesToRead = (int)input.Length;
                int numBytesRead = 0;
                int bytesread = input.Read(bytes, numBytesRead, bytes.Length);

                //Lendo o arquivo
                string legenda = encoding.GetString(bytes, numBytesRead, bytesread);

                var blocos = rgx.Matches(legenda);

                //Criando variaveis 
                string legendaOriginal, legendaReplace, legendamodificada = "";

                //Pegando todos os blocos e substituindo pelo bloco com deslocamento de tempo
                for (int i = 0; i < blocos.Count; i++)
                {
                    legendaOriginal = blocos[i].Value;

                    legendaReplace = String.Format("{0}\r\n{1:HH\\:mm\\:ss\\.fff} --> {2:HH\\:mm\\:ss\\.fff}\r\n", blocos[i].Groups["sequence"].Value,
                          DateTime.Parse(blocos[i].Groups["start"].Value.Replace(",", ".")).AddSeconds(timeSpan.TotalSeconds),
                          DateTime.Parse(blocos[i].Groups["end"].Value.Replace(",", ".")).AddSeconds(timeSpan.TotalSeconds));

                    legendamodificada = legendamodificada + rgx.Replace(legendaOriginal, legendaReplace) + blocos[i].Groups[4];
                }


                //Escrevendo o arquivo novo
                output.Write(encoding.GetBytes(legendamodificada), 0, encoding.GetByteCount(legendamodificada));


            }
            catch
            {
                throw new NotImplementedException();
            }


      
        }

       
    }
}
