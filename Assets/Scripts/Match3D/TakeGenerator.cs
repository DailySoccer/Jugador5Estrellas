// #define REGISTER_GAME_EVENTS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;
using FootballStar.Common;
using FootballStar.Audio;
using System.Linq;
using ExtensionMethods;

namespace FootballStar.Match3D {
    public class TakeGenerator
    {
        static Vector2 goal = new Vector2(52.5f, 0);
        static Vector2 Move(Vector2 pos, float time, Vector2 dst) {
            return pos + new Vector2(dst.x - pos.x, dst.y - pos.y).normalized * time * 7.0f;
        }

        static Vector2 Advance(string name, Vector2 pos, float time, Vector2 dst, float delay=0.0f) {
            Vector2 tmp = Move(pos, time, dst);
            Actions.Add(new AnimatorTimeline.JSONAction()
            {
                method = "moveto",
                go = name,
                delay = delay,
                time = time,
                easeType = 21,
                customEase = new float[0],
                path = new AnimatorTimeline.JSONVector3[] {
                        new AnimatorTimeline.JSONVector3() { x = tmp.x, y=0.0f, z = tmp.y }
                    }
            });
            return tmp;
        }

        public static AnimatorTimeline.JSONTake Generate(InteractiveType type)
        {
            var take = new AnimatorTimeline.JSONTake() {
                takeName = type.ToString()
            };
            switch (type) {
                case InteractiveType.Pass:
                    GeneratePass(take);
                    break;
                case InteractiveType.Shot:
                    GenerateShot(take);
                    break;
                case InteractiveType.Dribling:
                    GenerateDribling(take);
                    break;
            }
            return take;
        }

        static Vector2[] Home = new Vector2[11];
        static Vector2[] Away = new Vector2[11];
        static AnimatorTimeline.JSONQuaternion[] RHome = new AnimatorTimeline.JSONQuaternion[11];
        static AnimatorTimeline.JSONQuaternion[] RAway = new AnimatorTimeline.JSONQuaternion[11];
        const int Prota = 6;

        static Vector2 Dimesiones = new Vector2(52.5f/4.0f, 34f/2.0f); // 13.12f, 17f
        static void GenerateDefaultPosition(bool random=true)
        {
            // Defensa. No juegan.
            Home[1] = new Vector2(Dimesiones.x * -1.5f, Dimesiones.y * -1.5f);
            Home[2] = new Vector2(Dimesiones.x * -1.5f, Dimesiones.y * -0.5f);
            Home[3] = new Vector2(Dimesiones.x * -1.5f, Dimesiones.y * +0.5f);
            Home[4] = new Vector2(Dimesiones.x * -1.5f, Dimesiones.y * +1.5f);

            // Media.
            Home[5] = new Vector2(Dimesiones.x * +0.5f, Dimesiones.y * -1.5f);
            Home[6] = new Vector2(Dimesiones.x * +0.5f, Dimesiones.y * -0.5f);
            Home[7] = new Vector2(Dimesiones.x * +0.5f, Dimesiones.y * +0.5f);
            Home[8] = new Vector2(Dimesiones.x * +0.5f, Dimesiones.y * +1.5f);
            // Delateros
            Home[9] = new Vector2(Dimesiones.x * +2.4f, Dimesiones.y * -0.5f);
            Home[10] = new Vector2(Dimesiones.x * +2.4f, Dimesiones.y * +0.5f);


            // Defensa.
            Away[1] = new Vector2(Dimesiones.x * +2.5f, Dimesiones.y * -1.5f);
            Away[2] = new Vector2(Dimesiones.x * +2.6f, Dimesiones.y * -0.5f);
            Away[3] = new Vector2(Dimesiones.x * +2.6f, Dimesiones.y * +0.5f);
            Away[4] = new Vector2(Dimesiones.x * +2.5f, Dimesiones.y * +1.5f);
            // Media.
            Away[5] = new Vector2(Dimesiones.x * +1.5f, Dimesiones.y * -1.5f);
            Away[6] = new Vector2(Dimesiones.x * +1.5f, Dimesiones.y * -0.5f);
            Away[7] = new Vector2(Dimesiones.x * +1.5f, Dimesiones.y * +0.5f);
            Away[8] = new Vector2(Dimesiones.x * +1.5f, Dimesiones.y * +1.5f);
            // Delateros
            Away[9] = new Vector2(Dimesiones.x * +0.5f, Dimesiones.y * -0.5f);
            Away[10] = new Vector2(Dimesiones.x * +0.5f, Dimesiones.y * +0.5f);
            if (random)
            {
                for (int i = 1; i < 11; ++i)
                {
                    Home[i] += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                    Away[i] += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                }
            }

        }

        static void MakeInits(AnimatorTimeline.JSONTake take) {
            // Aqui podemos crear las jugadas dinamicamente.
            // Solo necesito posicionar a los juagadores y añadirles las acciones.
            List<AnimatorTimeline.JSONInit> inits = new List<AnimatorTimeline.JSONInit>();
            for (int i = 1; i < 11; ++i)
            {
                inits.Add(new AnimatorTimeline.JSONInit()
                {
                    type = "position",
                    go = "Local" + (i + 1),
                    position = new AnimatorTimeline.JSONVector3() { x = Home[i].x, y = 0.0f, z = Home[i].y }
                });
                if (RHome[i] != null)
                {
                    inits.Add(new AnimatorTimeline.JSONInit()
                    {
                        type = "rotation",
                        go = "Local" + (i + 1),
                        rotation = RHome[i]
                    });
                }

                inits.Add(new AnimatorTimeline.JSONInit()
                {
                    type = "position",
                    go = "Visitante" + (i + 1),
                    position = new AnimatorTimeline.JSONVector3() { x = Away[i].x, y = 0.0f, z = Away[i].y }
                });
                if (RAway[i] != null)
                {
                    inits.Add(new AnimatorTimeline.JSONInit()
                    {
                        type = "rotation",
                        go = "Visitante" + (i + 1),
                        rotation = RAway[i]
                    });
                }
            }
            inits.Add(new AnimatorTimeline.JSONInit() {
                type = "position",
                go = "Balon",
                position = new AnimatorTimeline.JSONVector3() { x = Home[Prota].x, y = 0.0f, z = Home[Prota].y }
            });
            take.inits = inits.ToArray();

            Actions.Clear();
            Actions.Add(new AnimatorTimeline.JSONAction()
            {
                method = "sendmessage",
                go = "Local" + (Prota + 1),
                delay = 0,
                strings = new string[] { "Balon" }
            });
        }
        static List<AnimatorTimeline.JSONAction> Actions = new List<AnimatorTimeline.JSONAction>();

        public static void GeneratePass(AnimatorTimeline.JSONTake take) {
            // Catalogamos los pases en 2 lineas, pase desde defensa y pase desde medio campo.
            // Esto define quien hace el pase, si la defensa o 
            GenerateDefaultPosition();
            // Elegimos desde que puesto hacemos el pase, tenemos 4 medios, osea que le ponemos en la posicion que estimamos oportuna.
            int pos = 5 + UnityEngine.Random.Range(0, 4);
            Vector2 tmp = Home[Prota];
            Home[Prota] = Home[pos];
            Home[pos] = tmp;

            // Acerco a los delanteros hacia la pelota.
            Home[9] = Move(Home[9], Random.Range(1.75f,2.5f), Home[Prota]);
            Home[10] = Move(Home[10], Random.Range(1.75f, 2.5f), Home[Prota]);

            MakeInits(take);

            Advance("Local6", Home[5], Random.Range(1f,2f), goal);
            Advance("Local7", Home[6], Random.Range(1f, 2f), goal);
            Advance("Local8", Home[7], Random.Range(1f, 2f), goal);
            Advance("Local9", Home[8], Random.Range(1f, 2f), goal);

            Advance("Local10", Home[9], Random.Range(0.5f, 1.25f), goal); // Y los mando hacia la porteria.
            Advance("Local11", Home[10], Random.Range(0.5f, 1.25f), goal);

            // Hago que los Medios y defensas contrarios cubran.
            Advance("Visitante2", Away[1], Random.Range(0.75f, 1.5f), goal);
            Advance("Visitante3", Away[2], Random.Range(0.75f, 1.5f), goal);
            Advance("Visitante4", Away[3], Random.Range(0.75f, 1.5f), goal);
            Advance("Visitante5", Away[4], Random.Range(0.75f, 1.5f), goal);

            Advance("Visitante6", Away[5], Random.Range(0.25f, 0.75f), goal);
            Advance("Visitante7", Away[6], Random.Range(0.25f, 0.75f), goal);
            Advance("Visitante8", Away[7], Random.Range(0.25f, 0.75f), goal);
            Advance("Visitante9", Away[8], Random.Range(0.25f, 0.75f), goal);

            // El pase se realizara a los delanteros.  Vamos a mandarles a una buena posición.
            Actions.Add(new AnimatorTimeline.JSONAction() {
                method = "invokemethod",
                go = "Local" + (Prota + 1),
                delay = 2.0f,
                strings = new string[] { "Entrenador", "Pase" }
            });

            take.actions = Actions.ToArray();
        }
        public static void GenerateShot(AnimatorTimeline.JSONTake take) {
            // Catalogamos los pases en 2 lineas, pase desde defensa y pase desde medio campo.
            // Esto define quien hace el pase, si la defensa o 
            GenerateDefaultPosition();
            // Elegimos desde que puesto hacemos el tiro, tenemos 2 delanteros.
            int pos = 9 + UnityEngine.Random.Range(0, 2);
            Vector2 tmp = Home[Prota];
            Home[Prota] = Home[pos];
            Home[pos] = tmp;

            float disp = Random.Range(-0.75f, 0.25f);

            Home[Prota] = Move( Home[Prota], disp, goal );
            // Quizas, desplazar hacia un lado al protagonista.
            MakeInits(take);

            // Y los mando hacia la porteria.
            if (pos != 9) Advance("Local10", Home[9], Random.Range(0.5f, 1.25f), goal);
            if (pos != 10) Advance("Local11", Home[10], Random.Range(1.0f, 2.25f), goal);
            disp += 2;
            if (disp < 0.5f) disp = 0.5f;
            else if (disp > 1) disp = 1;


            Vector2 end = Advance("Local"+(Prota+1), Home[Prota], disp+2, goal);
            
            // Hago que los Medios y defensas contrarios cubran.
            Advance("Visitante2", Away[1], Random.Range(1.75f, 2.6f), goal);
            Advance("Visitante3", Away[2], Random.Range(1.75f, 2.0f), goal);
            Advance("Visitante4", Away[3], Random.Range(1.75f, 2.0f), goal);
            Advance("Visitante5", Away[4], Random.Range(1.75f, 2.6f), goal);

            Advance("Visitante6", Away[5], Random.Range(1.25f, 3.0f), goal);
            Advance("Visitante7", Away[6], Random.Range(1.25f, 3.0f), goal);
            Advance("Visitante8", Away[7], Random.Range(1.25f, 3.0f), goal);
            Advance("Visitante9", Away[8], Random.Range(1.25f, 3.0f), goal);

            // Chut a puerta.
            Actions.Add(new AnimatorTimeline.JSONAction() {
                method = "sendmessage",
                go = "Local" + (Prota + 1),
                delay = 2.0f,
                strings = new string[] { "Chut" },
                eventParams = new AnimatorTimeline.JSONEventParameter[] {
                        new AnimatorTimeline.JSONEventParameter(){
                            valueType=2,
                            val_float=1f
                        }
                    }
            });

            take.actions = Actions.ToArray();
        }
        public static void GenerateDribling(AnimatorTimeline.JSONTake take) {
            // Catalogamos los pases en 2 lineas, pase desde defensa y pase desde medio campo.
            // Esto define quien hace el pase, si la defensa o 
            GenerateDefaultPosition();
            // Elegimos desde que puesto hacemos el pase, tenemos 4 medios, osea que le ponemos en la posicion que estimamos oportuna.
            int pos = 5 + UnityEngine.Random.Range(0, 4);
            Vector2 tmp = Home[Prota];
            Home[Prota] = Home[pos];
            Home[pos] = tmp;

            Vector2 DriblingPos = Move(Home[Prota], 1.3f, goal);
            Away[pos] = Move(DriblingPos, 1.5f, Away[pos]);
            Debug.DrawLine(new Vector3(Home[Prota].x, 0, Home[Prota].y), new Vector3(DriblingPos.x, 0, DriblingPos.y), Color.red, 10);

            // Encara a destino para que no se entretenga.
            Quaternion q = Quaternion.LookRotation((DriblingPos - Home[Prota]).normalized);
            RHome[Prota] = new AnimatorTimeline.JSONQuaternion() { x = q.x, y = q.y, z = q.z, w = q.w };

            q = Quaternion.LookRotation((DriblingPos - Away[pos]).normalized);
            RAway[pos] = new AnimatorTimeline.JSONQuaternion() { x = q.x, y = q.y, z = q.z, w = q.w };

            // Seguro que tengo que poner en rango a prota y a marcador.

            MakeInits(take);

            for (int i = 5; i <= 10; ++i) {
                if (i != Prota)
                    Advance("Local" + (i + 1), Home[i], Random.Range(1f, 2f), goal);
                else
                    Advance("Local" + (i + 1), Home[i], 2, goal);
            }

            for (int i = 1; i <= 8; ++i) {
                float t = i < 5 ? Random.Range(0.75f, 1.5f) : Random.Range(0.25f, 0.75f);
                if(i!= pos)
                    Advance("Visitante"+(i+1), Away[i], t, goal);
                else
                    Advance("Visitante" + (i + 1), Away[i], 2f, DriblingPos);
            }

            // El pase se realizara a los delanteros.  Vamos a mandarles a una buena posición.
            Actions.Add(new AnimatorTimeline.JSONAction() {
                method = "sendmessage",
                go = "Local" + (Prota + 1),
                delay = 2.2f,
                strings = new string[] { "RegatearA" },
                eventParams = new AnimatorTimeline.JSONEventParameter[] {
                    new AnimatorTimeline.JSONEventParameter() {
                        valueType=11,
                        val_obj="Visitante"+(pos+1)
                    }
                }
            });

            Advance("Local" + (Prota + 1), DriblingPos, 2, goal, 2.1f);

            take.actions = Actions.ToArray();
        }
    }
}


