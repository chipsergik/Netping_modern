function CompareJs(table) {
    this._table = $(table);
}

CompareJs.prototype.showDistinct = function(){
    var rows = $('tbody tr', this._table);
    if (rows.length > 0)
    {
        for(var i = 0; i < rows.length; i++)
        {
            var row = rows.eq(i);
            var columns = $('td', row);
            var value = null;
            var distinct = false;
            for(var j = 1; j < columns.length; j++)
            {
                var column = columns.eq(j);
                if (value == null)
                {
                    value = column.text().trim().toLowerCase();
                }
                else
                {
                    if (value != column.text().trim().toLowerCase())
                    {
                        distinct = true;
                        break;
                    }
                }
            }
            if (!distinct)
            {
                row.fadeOut(500);
            }
        }
    }
};

CompareJs.prototype.showAll = function() {
    $('tbody tr:hidden', this._table).fadeIn(500);
};
