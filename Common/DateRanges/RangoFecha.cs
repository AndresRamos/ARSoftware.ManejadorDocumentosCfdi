﻿using System;

namespace Common.DateRanges
{
    public class RangoFecha
    {
        public static readonly RangoFecha Hoy = new RangoFecha(DateTime.Today, DateTime.Today);
        public static readonly RangoFecha Ayer = new RangoFecha(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));

        public static readonly RangoFecha EstaSemana =
            new RangoFecha(DateTime.Today.FirstDayOfWeek(), DateTime.Today.FirstDayOfWeek().AddDays(6));

        public static readonly RangoFecha EstaSemanaAlDia = new RangoFecha(DateTime.Today.FirstDayOfWeek(), DateTime.Today);
        public static readonly RangoFecha EsteMes = new RangoFecha(DateTime.Today.FirstDayOfMonth(), DateTime.Today.LastDayOfMonth());
        public static readonly RangoFecha EsteMesAlDia = new RangoFecha(DateTime.Today.FirstDayOfMonth(), DateTime.Today);

        public static readonly RangoFecha EsteAno =
            new RangoFecha(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 31));

        public static readonly RangoFecha EsteAnoAlDia = new RangoFecha(new DateTime(DateTime.Today.Year, 1, 1), DateTime.Today);
        public static RangoFecha[] BuscarTodos = { Hoy, Ayer, EstaSemana, EsteMes, EsteAno };

        private RangoFecha(DateTime inicio, DateTime fin)
        {
            Inicio = inicio;
            Fin = fin;
        }

        public DateTime Inicio { get; }
        public DateTime Fin { get; }
    }
}
