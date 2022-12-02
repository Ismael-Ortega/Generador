//Ortega Espinosa Angel Ismael
using System;
using System.Collections.Generic;

//Requerimiento 1.- Construir un metodo para escribir en el archivo lenguaje.cs indentando el codigo
//                  { -> incrementa un tabluador, } -> decrementa un tabulador
//Requerimiento 2.- Declarar un atributo "primeraProduccion" de tipo string y actualizarlo con la
//                  primera produccion de la gramatica LISTO
//Requerimiento 3.- La primera produccion es publica y el resto privadas LISTO
//Requerimiento 4.- El constructor Lexico parametrizado debe validar que la extension del archivo a compilar
//                  sea .gen, y si no es, entonces levantamos una excepcion LISTO
//Requerimiento 5.- Resolver la ambiguedad de ST y SNT
//                  recorrer linea por linea el archivo .gram para extraer el nombre de cada produccion LISTO
//Requerimiento 6.- Agregar el parenetsis izquierdo y derecho escapados en la matriz de transiciones LISTO
//Requerimiento 7.- Implementar el OR y la cerradura epsilon

namespace Generador
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        List<string> listaSNT;
        string primeraProduccion, produccion;
        int contTabulado = 0, tab = 0, tam = 0;
        bool producciones = false;

        public Lenguaje(string nombre) : base(nombre)
        {
            listaSNT = new List<string>();
            primeraProduccion = "";
            produccion = "";
        }

        public Lenguaje()
        {
            listaSNT = new List<string>();
            primeraProduccion = "";
            produccion = "";
        }
        public void Dispose()
        {
            cerrar();
        }
        private bool esSNT(string contenido)
        {
            return listaSNT.Contains(contenido);
        }
        private void agregarSNT()
        {
            int posicionAux = posicion;
            int lineaAux = linea;
            int tamañoAux = getContenido().Length;

            while (!FinArchivo()){
                produccion = getContenido();
                listaSNT.Add(produccion);
                archivo.ReadLine();
                NextToken();
                
            } 
                posicion = posicionAux - tamañoAux;
                linea = lineaAux;
                setPosicion(posicion);
                NextToken();
            
        }
        private void setPosicion(int posicion) //Creamos metodo para poder guardar la posicion
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }
       /* public void tabluador(string texto)
        { //Requerimiento 1 Crear un metodo para el identado del archivo
            //Debe de recorrer toda la cadena en busca de llaves
            tam = getContenido().Length;
            for (int i = 0; i < tam; i++)
            {
                if (getContenido[i] == '{')
                {
                    contTabulado++;
                }
                else if (getContenido[i] == '}')
                {
                    contTabulado--;
                }
            }
            for (tab = 0; tab < contTabulado; tab++)
            {
                programa.WriteLine("\t");
            }
        }*/
        private void Programa(string primeraProduccion)
        {
            //tabluador("using System;");
            programa.WriteLine("using System;");
            programa.WriteLine("using System.IO;");
            programa.WriteLine("using System.Collections.Generic;");
            programa.WriteLine();
            programa.WriteLine("namespace Generico");
            programa.WriteLine("{");
            programa.WriteLine("\tpublic class Programa");
            programa.WriteLine("\t{");
            programa.WriteLine("\t\tstatic void Main(string[] args)");
            programa.WriteLine("\t\t{");
            //programa.WriteLine("\t\t\t{");
            programa.WriteLine("\t\t\ttry");
            programa.WriteLine("\t\t\t{");
            programa.WriteLine("\t\t\t\tusing (Lenguaje a = new Lenguaje())");
            programa.WriteLine();
            programa.WriteLine("\t\t\t\t{");
            programa.WriteLine("\t\t\t\t\ta." + primeraProduccion + "();");
            programa.WriteLine("\t\t\t\t}");
            programa.WriteLine("\t\t\t}");
            programa.WriteLine("\t\t\tcatch (Exception e)");
            programa.WriteLine("\t\t\t{");
            programa.WriteLine("\t\t\t\tConsole.WriteLine(e.Message);");
            programa.WriteLine("\t\t\t}");
            programa.WriteLine("\t\t}");
            programa.WriteLine("\t}");
            programa.WriteLine("}");
        }
        public void gramatica()
        {
            cabecera();
            agregarSNT();
            
            //Requerimiento 2.- Declarar un atributo "primeraProduccion" de tipo string y actualizarlo
            primeraProduccion = getContenido();
            Programa(primeraProduccion);
            cabeceraLenguaje();
            listaProducciones();
            lenguaje.WriteLine("\t}");
            lenguaje.WriteLine("}");

        }
        private void cabecera()
        {
            match("Gramatica");
            match(":");
            match(Tipos.ST);
            match(Tipos.finProduccion);
        }
        private void cabeceraLenguaje()
        {
            lenguaje.WriteLine("using System;");
            lenguaje.WriteLine("using System.Collections.Generic;");

            lenguaje.WriteLine("namespace Generico");
            lenguaje.WriteLine("\t{");
            lenguaje.WriteLine("\tpublic class Lenguaje : Sintaxis, IDisposable");
            lenguaje.WriteLine("\t{");
            lenguaje.WriteLine("\t\tpublic Lenguaje(string nombre) : base(nombre)");
            lenguaje.WriteLine("\t\t{");
            lenguaje.WriteLine("\t\t}");

            lenguaje.WriteLine("\t\tpublic Lenguaje()");
            lenguaje.WriteLine("\t\t{");
            lenguaje.WriteLine("\t\t}");
            lenguaje.WriteLine("\t\tpublic void Dispose()");
            lenguaje.WriteLine("\t\t{");
            lenguaje.WriteLine("\t\t\tcerrar();");
            lenguaje.WriteLine("\t\t}");
        }
        private void listaProducciones()
        {
            //Requerimiento 3.- La primera produccion es publica y el resto privadas
            if (producciones == false)
            {
                //Requerimiento 2.- Declarar un atributo "primeraProduccion" de tipo string y actualizarlo
                primeraProduccion = getContenido();
                lenguaje.WriteLine("\t\tpublic void " + primeraProduccion + "()");
                producciones = true;
            }
            else
            {
                lenguaje.WriteLine("\t\tprivate void " + getContenido() + "()");
            }
            lenguaje.WriteLine("\t\t{");
            match(Tipos.ST);
            match(Tipos.Produce);
            simbolos();
            match(Tipos.finProduccion);
            lenguaje.WriteLine("\t\t}");
            if (!FinArchivo())
            {
                listaProducciones();
            }
        }
        private void simbolos()
        {
            if (getContenido() == "\\(")
            {
                match("\\(");
                lenguaje.WriteLine("\t\tif ()");
                lenguaje.WriteLine("\t\t{");
                simbolos();
                match("\\)");
                lenguaje.WriteLine("\t\t}");
            }
            else if (esTipo(getContenido()))
            {
                lenguaje.WriteLine("\t\t\tmatch(Tipos." + getContenido() + ");");
                match(Tipos.ST);
            }
            else if (esSNT(getContenido()))
            {
                lenguaje.WriteLine("\t\t\t" + getContenido() + "();");
                match(Tipos.ST);
            }
            else if (getClasificacion() == Tipos.ST)
            {
                //Debe de entrar aqui el match("#");
                lenguaje.WriteLine("\t\t\tmatch(\"" + getContenido() + "\");");
                match(Tipos.ST);
            }
            else
            {
                throw new Exception("Error de sintaxis");
            }

            if (getClasificacion() != Tipos.finProduccion && getContenido() != "\\)")
            {
                simbolos();
            }
        }

        private bool esTipo(string clasificacion)
        {
            switch (clasificacion)
            {
                case "Identificador":
                case "Numero":
                case "Caracter":
                case "Asignacion":
                case "Inicializacion":
                case "OperadorLogico":
                case "OperadorRelacional":
                case "OperadorTernario":
                case "OperadorTermino":
                case "OperadorFactor":
                case "IncrementoTermino":
                case "IncrementoFactor":
                case "FinSentencia":
                case "Cadena":
                case "TipoDato":
                case "Zona":
                case "Condicion":
                case "Ciclo":
                    return true;
            }
            return false;
        }
    }
}