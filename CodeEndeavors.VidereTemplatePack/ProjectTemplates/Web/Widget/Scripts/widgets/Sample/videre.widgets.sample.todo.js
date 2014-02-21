videre.registerNamespace('$clientnamespace$');

$clientnamespace$.todo = videre.widgets.base.extend(
{
    get_data: function() { return this._data; },
    set_data: function(v)
    {
        this._data = v;
        this._dataDict = v.toDictionary(function(d) { return d.Id; });
    },

    //constructor
    init: function()
    {
        this._base();  //call base method

        //member variables
        this._data = null;
        this._dataDict = null;
        this._selectedItem = null;

        this._dialog = null;

        //delegates that we will use more than once should be stored in member variable to minimize memory useage/leaks
        this._delegates = {
            onDataReturn: videre.createDelegate(this, this._onDataReturn),
            onDataSaveReturn: videre.createDelegate(this, this._onDataSaveReturn),
            onActionClicked: videre.createDelegate(this, this._onActionClicked)
        };
    },

    //note:  _onload is only event handler we order near the top
    _onLoad: function(src, args)
    {
        this._base(); //call base method

        //hook up our controls onload
        this._dialog = this.getControl('Dialog').modal('hide');
        this.getControl('btnSave').click(videre.createDelegate(this, this._onSaveClicked));
        this.getControl('btnNew').click(videre.createDelegate(this, this._onNewClicked));

        if (this._data != null) //since we passed the data on the initial render, no need to use ajax to fetch again
            this.bind();
        else
            this.refresh();
    },

    //public methods
    refresh: function()
    {
        this.ajax('~/$servernamespace$/ToDo/GetTasks', {}, this._delegates.onDataReturn);
    },

    bind: function()
    {
        videre.dataTables.clear(this.getControl('ItemTable'));
        this.getControl('ItemList').html(this.getControl('ItemListTemplate').render(this._data));
        this.getControl('ItemList').find('.btn').click(this._delegates.onActionClicked);
        videre.dataTables.bind(this.getControl('ItemTable'), { aoColumns: [{ bSortable: false }] });
    },

    reset: function()
    {
        this.clearMsgs();
        this.clearMsgs(this._dialog);
    },

    newItem: function()
    {
        this._selectedItem = { Name: '', Description: '', CompleteDate: null };
        this.edit();
    },

    edit: function()
    {
        if (this._selectedItem != null)
        {
            this.reset();
            videre.UI.showModal(this._dialog);
            this.bindData(this._selectedItem, this._dialog);
        }
    },

    save: function()
    {
        if (this.validControls(this._dialog, this._dialog))
        {
            var item = this.persistData(this._selectedItem, true, this._dialog);

            if (item.Sequence == '')
                item.Sequence = null;

            item.Type = new Number(item.Type);
            item.LoadType = new Number(item.LoadType);

            this.ajax('~/$servernamespace$/ToDo/SaveTask', { task: item }, this._delegates.onDataSaveReturn, null, this._dialog);
        }
    },

    deleteItem: function(id)
    {
        //if (confirm('Are you sure you wish to remove this entry?')) //todo: localize?
        var self = this;
        videre.UI.confirm('Delete Entry', 'Are you sure you wish to remove this entry?', function ()
        {
            self.ajax('~/$servernamespace$/ToDo/DeleteTask', { id: id }, self._delegates.onDataSaveReturn);
        });
    },

    //private methods
    _handleAction: function(action, id)
    {
        this._selectedItem = this._dataDict[id];
        if (this._selectedItem != null)
        {
            if (action == 'edit')
                this.edit();
            else if (action == 'delete')
                this.deleteItem(id);
        }
    },

    //event handlers (private)
    _onDataReturn: function(result, ctx)
    {
        if (!result.HasError)
        {
            this.set_data(result.Data);
            this.bind();
        }
    },

    _onDataSaveReturn: function(result)
    {
        if (!result.HasError && result.Data)
        {
            this.refresh();
            this._dialog.modal('hide');
        }
    },

    _onActionClicked: function(e)
    {
        var ctl = $(e.target).closest('[data-action]');
        this._handleAction(ctl.data('action'), ctl.data('id'));
    },

    _onSaveClicked: function(e)
    {
        this.save();
    },

    _onNewClicked: function(e)
    {
        this.newItem();
    }


});

