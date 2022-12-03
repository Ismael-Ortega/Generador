//Ortega Espinosa Angel Ismael
using System;
using System.Collections.Generic;

//Requerimiento 1.- Construir un metodo para escribir en el archivo lenguaje.cs indentando el codigo
//                  { -> incrementa un tabluador, } -> decrementa un tabulador LISTO?
//Requerimiento 2.- Declarar un atributo "primeraProduccion" de tipo string y actualizarlo con la
//                  primera produccion de la gramatica LISTO
//Requerimiento 3.- La primera produccion es publica y el resto privadas LISTO
//Requerimiento 4.- El constructor Lexico parametrizado debe validar que la extension del archivo a compilar
//                  sea .gen, y si no es, entonces levantamos una excepcion LISTO
//Requerimiento 5.- Resolver la ambiguedad de ST y SNT
//                  recorrer linea por linea el archivo .gram para extraer el nombre de cada produccion LISTO
//Requerimiento 6.- Agregar el parenetsis izquierdo y derecho escapados en la matriz de transiciones LISTO
//Requerimiento 7.- Implementar el OR y la cerradura epsilon LISTO?

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
            //Requerimiento 5.- Resolver la ambiguedad de ST y SNT
            int posicionAux = posicion;
            int lineaAux = linea;
            int tamañoAux = getContenido().Length;

            while (!FinArchivo())
            {
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
        public void tabulador(string texto, string doc)
        {
            //Requerimiento 1 Crear un metodo para el identado del archivo
            tam = texto.Length;
            for (int i = 0; i < tam; i++)
            {
                if (texto[i] == '}')
                {
                    contTabulado--;
                }
            }
            for (tab = 0; tab < contTabulado; tab++)
            {
                if (doc == "programa")
                {
                    programa.Write("\t");
                }
                else
                {
                    lenguaje.Write("\t");
                }
            }
            for (int i = 0; i < tam; i++)
            {
                if (texto[i] == '{')
                {
                    contTabulado++;
                }
            }
            if (doc == "programa")
            {
                programa.WriteLine(texto);
            }
            else
            {
                lenguaje.WriteLine(texto);
            }
        }
        private void Programa(string primeraProduccion)
        {
            tabulador("using System;","programa");
            tabulador("using System.IO;","programa");
            tabulador("using System.Collections.Generic;","programa");
            tabulador("","programa");
            tabulador("namespace Generico","programa");
            tabulador("{","programa");
            tabulador("public class Programa","programa");
            tabulador("{","programa");
            tabulador("static void Main(string[] args)","programa");
            tabulador("{","programa");
            tabulador("try","programa");
            tabulador("{","programa");
            tabulador("using (Lenguaje a = new Lenguaje())","programa");
            tabulador("","programa");
            tabulador("{","programa");
            tabulador("a." + primeraProduccion + "();","programa");
            tabulador("}","programa");
            tabulador("}","programa");
            tabulador("catch (Exception e)","programa");
            tabulador("{","programa");
            tabulador("Console.WriteLine(e.Message);","programa");
            tabulador("}","programa");
            tabulador("}","programa");
            tabulador("}","programa");
            tabulador("}","programa");
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
            tabulador("}", "lenguaje");
            tabulador("}", "lenguaje");

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
            tabulador("using System;", "lenguaje");
            tabulador("using System.Collections.Generic;", "lenguaje");

            tabulador("namespace Generico", "lenguaje");
            tabulador("{", "lenguaje");
            tabulador("public class Lenguaje : Sintaxis, IDisposable", "lenguaje");
            tabulador("{", "lenguaje");
            tabulador("public Lenguaje(string nombre) : base(nombre)", "lenguaje");
            tabulador("{", "lenguaje");
            tabulador("}", "lenguaje");

            tabulador("public Lenguaje()", "lenguaje");
            tabulador("{", "lenguaje");
            tabulador("}", "lenguaje");
            tabulador("public void Dispose()", "lenguaje");
            tabulador("{", "lenguaje");
            tabulador("cerrar();", "lenguaje");
            tabulador("}", "lenguaje");
        }
        private void listaProducciones()
        {
            //Requerimiento 3.- La primera produccion es publica y el resto privadas
            if (producciones == false)
            {
                //Requerimiento 2.- Declarar un atributo "primeraProduccion" de tipo string y actualizarlo
                primeraProduccion = getContenido();
                tabulador("public void " + primeraProduccion + "()", "lenguaje");
                producciones = true;
            }
            else
            {
                tabulador("private void " + getContenido() + "()", "lenguaje");
            }
            tabulador("{", "lenguaje");
            match(Tipos.ST);
            match(Tipos.Produce);
            simbolos();
            match(Tipos.finProduccion);
            tabulador("}", "lenguaje");
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
                //Requerimiento 7.- Implementar la cerradura Epsilon
                if (esSNT(getContenido()))
                {
                    throw new Exception("Error Sintactico: No se puede aplicar la cerradura Epsilon a un SNT");
                }
                else if (esTipo(getContenido()))
                {
                    tabulador("if (Tipos." + getContenido() + " == getClasificacion())", "lenguaje");
                }
                else
                {
                    tabulador("if (getContenido() == \"" + getContenido() + "\")", "lenguaje");
                }
                tabulador("{", "lenguaje");
                simbolos();
                match("\\)");
                tabulador("}", "lenguaje");
            }
            else if (esTipo(getContenido()))
            {
                tabulador("match(Tipos." + getContenido() + ");", "lenguaje");
                match(Tipos.ST);
            }
            else if (esSNT(getContenido()))
            {
                tabulador("" + getContenido() + "();", "lenguaje");
                match(Tipos.ST);
            }
            else if (getClasificacion() == Tipos.ST)
            {
                tabulador("match(\"" + getContenido() + "\");", "lenguaje");
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