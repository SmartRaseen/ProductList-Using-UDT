using System;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedType(Format.UserDefined,
IsByteOrdered = true, MaxByteSize = 6000, ValidationMethodName ="ValidateProduct")]
public class Product: INullable, IBinarySerialize
{
    private bool is_Null;
    private int productID;
    private string productName;
    private int productQuantity;
    private decimal productPrice;
    public const int maxStringSize = 20;
    public bool IsNull
    {
        get
        {
            return is_Null;
        }
    }

    public static Product Null
    {
        get
        {
            Product product = new Product();
            product.is_Null = true;
            return product;
        }
    }

    public int ProductID
    {
        get
        {
            return this.productID;
        }
        set
        {
            int temp = productID;
            productID = value;

            if(!ValidateProduct())
            {
                productID = temp;
                throw new ArgumentException("Invalid ProductID");
            }
        }
    }

    public string ProductName
    {
        get
        {
            return this.productName;
        }
        set
        {
            string temp = productName;
            productName = value;

            if (!ValidateProduct())
            {
                productName = temp;
                throw new ArgumentException("Invalid ProductName");
            }
        }
    }

    public int ProductQuantity
    {
        get
        {
            return this.productQuantity;
        }
        set
        {
            int temp = productQuantity;
            productQuantity = value;

            if (!ValidateProduct())
            {
                productQuantity = temp;
                throw new ArgumentException("Invalid ProductQuantity");
            }
        }
    }

    public decimal ProductPrice
    {
        get
        {
            return this.productPrice;
        }
        set
        {
            decimal temp = productPrice;
            productPrice = value;

            if (!ValidateProduct())
            {
                productPrice = temp;
                throw new ArgumentException("Invalid ProductPrice");
            }
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(productID);
        builder.Append(",");
        builder.Append(productName);
        builder.Append(",");
        builder.Append(productQuantity);
        builder.Append(",");
        builder.Append(productPrice);
        return builder.ToString();
    }
    
    [SqlMethod(OnNullCall = false)]
    public static Product Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;
        else
        {
            Product product = new Product();
            string[] productData = s.Value.Split(",".ToCharArray());
            product.productID = int.Parse(productData[0]);
            product.productName = productData[1].ToString();
            product.productQuantity = int.Parse(productData[2]);
            product.productPrice = decimal.Parse(productData[3]);

            if (!product.ValidateAllProduct())
                throw new ArgumentException("Invalid Product Values...");
            return product;
        }
    }
    
    public bool ValidateProduct()
    {
        if ((productID > 0) || (productQuantity > 0) || (productPrice > 0) || !String.IsNullOrEmpty(productName))
            return true;
        
        else
            return false;
    }

    public bool ValidateAllProduct()
    {
         if((productID > 0) && (productQuantity > 0) && (productPrice > 0) && !String.IsNullOrEmpty(productName))
            return true;
         
         else
            return false;
    }

    public void Write(BinaryWriter w)
    {
        w.Write(productID);

        string Name = productName.PadRight(maxStringSize,'\0');
        for(int i=0;i<Name.Length;i++)
        {
            w.Write(Name[i]);
        }
 
        w.Write(productQuantity);
        w.Write(productPrice);
    }

    public void Read(BinaryReader r)
    {
        productID = r.ReadInt32();

        char[] chars = r.ReadChars(maxStringSize);

        int stringEnd = Array.IndexOf(chars,'\0');

        if (stringEnd == 0)
        {
            productName = null;
            return;
        }

        productName = new string(chars,0,stringEnd);

        productQuantity = r.ReadInt32();
        productPrice = r.ReadDecimal();
    }
}