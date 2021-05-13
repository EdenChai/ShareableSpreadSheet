using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

[Serializable]
class ShareableSpreadSheet
{
    public int m_rows;
    public int m_cols;
    public List<List<string>> my_SharableSpreadaheet;


    // construct a nRows*nCols spreadsheet
    public ShareableSpreadSheet(int nRows, int nCols)
    {
        m_rows = nRows;
        m_cols = nCols;
        my_SharableSpreadaheet = new List<List<string>>();

        for (int i = 0; i < m_rows; i++)
        {
            List<string> innerList = new List<string>();


            for (int j = 0; j < m_cols; j++)
            {
                string str = "" + i + "," + j;
                innerList.Add(str);
            }

            my_SharableSpreadaheet.Add(innerList);
        }

        for (int i = 0; i < my_SharableSpreadaheet.Count; i++)
        {
            for (int j = 0; j < my_SharableSpreadaheet[i].Count; j++)
            {
                Console.WriteLine(my_SharableSpreadaheet[i][j]);

            }
        }
    }


    // return the string at [row,col]
    public String getCell(int row, int col)
    {
        if (isValidCell(row, col))
        {
            return my_SharableSpreadaheet[row][col];
        }
        return null;
    }


    // set the string at [row,col]
    public bool setCell(int row, int col, String str)
    {
        if (isValidCell(row, col))
        {
            my_SharableSpreadaheet[row][col] = str;
            return true;
        }
        return false;
    }


    // search the cell with string str, and return true/false accordingly.
    // stores the location in row,col.
    // return the first cell that contains the string (search from first row to the last row)
    public bool searchString(String str, ref int row, ref int col)
    {
        if (isValidCell(row, col))
        {
            for (int i = 0; i < m_rows; i++)
            {
                for (int j = 0; j < m_cols; j++)
                {
                    if (String.Equals(my_SharableSpreadaheet[i][j], str))
                    {
                        row = i;
                        col = j;
                        return true;
                    }
                }
            }
        }
        return false;
    }


    // exchange the content of row1 and row2
    public bool exchangeRows(int row1, int row2)
    {
        if (row1 >= 0 && row1 < m_rows && row2 >= 0 && row2 < m_rows)
        {
            string temp = "";

            for (int i = 0; i < m_cols; i++)
            {
                temp = getCell(row2, i);
                setCell(row2, i, my_SharableSpreadaheet[row1][i]);
                setCell(row1, i, temp);

            }
        }
        return true;
    }


    // exchange the content of col1 and col2
    public bool exchangeCols(int col1, int col2)
    {
        if (col1 >= 0 && col1 < m_cols && col2 >= 0 && col2 < m_cols)
        {
            string temp = "";

            for (int i = 0; i < m_rows; i++)
            {
                temp = getCell(i, col2);
                setCell(i, col2, my_SharableSpreadaheet[i][col1]);
                setCell(i, col1, temp);

            }
        }
        return true;
    }


    // perform search in specific row
    public bool searchInRow(int row, String str, ref int col)
    {
        if (row >= 0 && row < m_rows)
        {
            for (int i = 0; i < m_cols; i++)
            {
                if (String.Equals(my_SharableSpreadaheet[row][i], str))
                {
                    col = i;
                    return true;
                }
            }
        }
        return false;
    }


    // perform search in specific col
    public bool searchInCol(int col, String str, ref int row)
    {
        if (col >= 0 && col < m_cols)
        {
            for (int i = 0; i < m_rows; i++)
            {
                if (String.Equals(my_SharableSpreadaheet[i][col], str))
                {
                    row = i;
                    return true;
                }
            }
        }
        return false;
    }


    // perform search within spesific range: [row1:row2,col1:col2] 
    //includes col1,col2,row1,row2
    public bool searchInRange(int col1, int col2, int row1, int row2, String str, ref int row, ref int col)
    {
        if (isValidCell(row1, col1) && isValidCell(row2, col2))
        {
            for (int i = row1; i <= row2; i++)
            {
                for (int j = col1; j <= col2; j++)
                {
                    if (String.Equals(my_SharableSpreadaheet[i][j], str))
                    {
                        row = i;
                        col = j;
                        return true;
                    }
                }
            }
        }
        return false;
    }


    //add a row after row1
    public bool addRow(int row1)
    {

        if (row1 >= 0 && row1 < m_rows)
        {
            List<string> colList = new List<string>();
            //initialize the cols in the row that we want to add
            for (int j = 0; j < m_cols; j++)
            {
                string str = "" + row1 + 1 + "," + j;
                colList.Add(str);
            }

            my_SharableSpreadaheet.Insert(row1 + 1, colList);
            m_rows++;
            return true;
        }
        return false;
    }


    //add a column after col1
    public bool addCol(int col1)
    {
        if (col1 >= 0 && col1 < m_cols)
        {
            for (int i = 0; i < m_rows; i++)
            {
                string str = "" + i + "," + col1 + 1;
                my_SharableSpreadaheet[i].Insert(col1 + 1, str);
                m_cols++;
                return true;
            }
        }
        return false;
    }


    // return the size of the spreadsheet in nRows, nCols
    public void getSize(ref int nRows, ref int nCols)
    {
        nRows = m_rows;
        nCols = m_cols;
    }


    // this function aims to limit the number of users that can perform the search operations concurrently.
    // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
    // In this case additional search operations will wait for existing search to finish.
    public bool setConcurrentSearchLimit(int nUsers)
    {
        return true;
    }


    // save the spreadsheet to a file fileName.
    // you can decide the format you save the data. There are several options.
    public bool save(String fileName)
    {
        //serialize
        using (Stream stream = File.Open(fileName, FileMode.Open))
        {
            var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            bformatter.Serialize(stream, this);
        }

        return true;
    }


    // load the spreadsheet from fileName
    // replace the data and size of the current spreadsheet with the loaded data
    public bool load(String fileName)
    {
        //deserialize
        using (Stream stream = File.Open(fileName, FileMode.Open))
        {
            var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            ShareableSpreadSheet MySharableSpreadaheet = (ShareableSpreadSheet)bformatter.Deserialize(stream);

        }
        return true;
    }

    private bool isValidCell(int row, int col)
    {
        return (row >= 0 && row < m_rows && col >= 0 && col < m_cols);
    }

    public static void Main(string[] args)
    {
        ShareableSpreadSheet sharable = new ShareableSpreadSheet(5, 5);
        Console.WriteLine(sharable.getCell(3, 3));
        sharable.setCell(1, 1, "Hi");
        Console.WriteLine(sharable.getCell(1, 1));
    }
}