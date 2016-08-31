using UnityEngine;
using System.Collections.Generic;

public class UsageChart
{
    private UsageInfo[][] chart;
    public int total;
    public int total_free;
    public int total_used
    {
        get
        {
            return total - total_free;
        }
    }



    public UsageChart(int dimension_x, int dimension_y)
    {
        chart = new UsageInfo[(int)dimension_x][];
        for (int x = 0; x < dimension_x; x++)
            chart[x] = new UsageInfo[(int)dimension_y];

        ClearChart();
        total = chart.Length * chart[0].Length;
    }

    public UsageChart(Vector2 dimensions)
    {
        chart = new UsageInfo[(int)dimensions.x][];
        for (int x = 0; x < dimensions.x; x++)
            chart[x] = new UsageInfo[(int)dimensions.y];

        ClearChart();
        total = chart.Length * chart[0].Length;
    }

    public void ClearChart()
    {
        for (int x = 0; x < chart.Length; x++)
            for (int y = 0; y < chart[0].Length; y++)
                chart[x][y] = UsageInfo.Free;
        total_free = chart.Length * chart[0].Length;
    }

    public UsageInfo Info(Vector2 position)
    {
        if (position.x < 0 || position.x >= chart.Length)
            return UsageInfo.Used;
        if (position.y < 0 || position.y >= chart[0].Length)
            return UsageInfo.Used;
        return chart[(int)position.x][(int)position.y];
    }

    public UsageInfo Info(int x, int y)
    {
        return chart[x][y];
    }

    public int Count(Vector2 position, Vector2 dimensions, UsageInfo look_for)
    {
        int to_return = 0;
        for (int x = (int)position.x; x < (int)(position.x + dimensions.x); x++)
            for (int y = (int)position.y; y < (int)(position.y + dimensions.y); y++)
                if (Info(x, y) == look_for)
                    to_return++;
        return to_return;
    }

    public Vector2[] GetAll(UsageInfo look_for)
    {
        List<Vector2> to_return = new List<Vector2>();
        for (int x = 0; x < chart.Length; x++)
            for (int y = 0; y < chart[0].Length; y++)
                if (Info(x, y) == look_for)
                    to_return.Add(new Vector2(x, y));
        return to_return.ToArray();
    }

    public Vector2 GetRandom(UsageInfo look_for)
    {
        Vector2[] list_all = GetAll(look_for);
        return list_all[(int)Random.Range(0, list_all.Length - 1)];
    }



    public void Free(Vector2 position)
    {
        Free((int)position.x, (int)position.y);
    }

    public void Free(int x, int y)
    {
        if (chart[x][y] == UsageInfo.Used)
            total_free++;
        chart[x][y] = UsageInfo.Free;
    }

    public void Free(Vector2 position, Vector2 dimensions)
    {
        for (int x = (int)position.x; x < (int)(position.x + dimensions.x); x++)
            for (int y = (int)position.y; y < (int)(position.y + dimensions.y); y++)
                Free(x, y);
    }




    public void Use(Vector2 position)
    {
        Use((int)position.x, (int)position.y);
    }

    public void Use(int x, int y)
    {
        if (x < 0 || x >= chart.Length || y < 0 || y >= chart[0].Length)
            return;
        if (chart[x][y] == UsageInfo.Free)
            total_free--;
        chart[x][y] = UsageInfo.Used;
    }
    
    public void Use(Vector2 position, Vector2 dimensions)
    {
        for (int x = (int)position.x; x < (int)(position.x + dimensions.x); x++)
            for (int y = (int)position.y; y < (int)(position.y + dimensions.y); y++)
                Use(x, y);
    }






    public override string ToString()
    {
        string to_return = "UsageChart: ";
        to_return += "\n Total: " + total;
        to_return += "\n " + total_free + " free, " + total_used + " used.";
        return to_return;
    }
}