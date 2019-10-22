using System.Linq;
using System.Collections.Generic;
using System;

namespace Graph
{
    public interface IGraph<T>
    {
        List<string> RoutesBetween(T source, T target);

    }

    public class Graph<T> : IGraph<T>
    {

        

        public Graph(IEnumerable<ILink<T>> links)
        {
            _links = links;
        }

        private IEnumerable<ILink<T>> _links;
        private List<string> _ways;

        public List<string> RoutesBetween(T source, T target)
        {
            //Criando Lista de String 
            _ways = new List<string>();

            //Void principal para encontrar o caminho
            FindWays(source, target, source.ToString());

            return _ways;
        }

       

        public void FindWays(T source, T target, string ponto = "")
        {
            try
            {
                //Cria uma lista com possiveis caminhos partindo do Source  
                var rotasPossiveis = _links.Where(x => x.Source.Equals(source) && !ponto.Contains(x.Target.ToString())).ToList();

                 
                foreach (var item in rotasPossiveis)
                {
                     //Recusividade para achar os caminhos
                    FindWays(item.Target, target, ponto + "-" + item.Target.ToString());
                }

                //Verificação
                if (ponto.Substring(ponto.Length - 1).Equals(target))
                {
                    _ways.Add(ponto);
                    ponto = "";
                }
            }
            catch
            {
                throw new NotImplementedException();
            }



        }
    }
}