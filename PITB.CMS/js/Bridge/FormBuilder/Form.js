Bridge.assembly("BridgeClassLib", function ($asm, globals) {
    "use strict";

    Bridge.define("BridgeClassLib.Utility", {
        statics: {
            methods: {
                IsSubstringPresent: function (listPermissions, str) {
                    return (System.Linq.Enumerable.from(listPermissions).firstOrDefault(function (n) {
                            return System.String.contains(n.Key,str);
                        }, null) != null);
                },
                IsStringPresent: function (listPermissions, str) {
                    return (System.Linq.Enumerable.from(listPermissions).firstOrDefault(function (n) {
                            return System.String.equals(n.Key, str);
                        }, null) != null);
                }
            }
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.DynamicForm", {
        fields: {
            Fields: null
        },
        ctors: {
            ctor: function () {
                this.$initialize();
                this.Fields = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.FormField)).ctor();
            },
            $ctor2: function (dynamicForm) {
                var $t, $t1, $t2, $t3;
                BridgeDTO.Form.DynamicForm.DynamicForm.ctor.call(this);

                var obj = dynamicForm;

                var rawFields = obj.Fields;
                this.Fields = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.FormField)).ctor();
                var count = 0;
                $t = Bridge.getEnumerator(rawFields);
                try {
                    while ($t.moveNext()) {
                        var rawField = $t.Current;

                        if (Bridge.referenceEquals(rawField.Kind, BridgeDTO.Form.DynamicForm.FormFieldType.RadioList)) {
                            var radioField = ($t1 = new BridgeDTO.Form.DynamicForm.RadioFormField(), $t1.Id = rawField.Id, $t1.Label = rawField.Label, $t1);
                            var listOption = rawField.Options;

                            radioField.Options = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.Option)).ctor();
                            $t1 = Bridge.getEnumerator(listOption);
                            try {
                                while ($t1.moveNext()) {
                                    var option = $t1.Current;
                                    radioField.Options.add(($t2 = new BridgeDTO.Form.DynamicForm.Option(), $t2.Value = option.Value, $t2.Text = option.Text, $t2));
                                }
                            } finally {
                                if (Bridge.is($t1, System.IDisposable)) {
                                    $t1.System$IDisposable$Dispose();
                                }
                            }
                            this.Fields.add(radioField);

                        } else if (Bridge.referenceEquals(rawField.Kind, BridgeDTO.Form.DynamicForm.FormFieldType.OptionList)) {
                            var optionField = ($t2 = new BridgeDTO.Form.DynamicForm.OptionsFormField(), $t2.Id = rawField.Id, $t2.Label = rawField.Label, $t2);
                            var listOption1 = rawField.Options;

                            optionField.Options = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.Option)).ctor();
                            $t2 = Bridge.getEnumerator(listOption1);
                            try {
                                while ($t2.moveNext()) {
                                    var option1 = $t2.Current;
                                    optionField.Options.add(($t3 = new BridgeDTO.Form.DynamicForm.Option(), $t3.Value = option1.Value, $t3.Text = option1.Text, $t3));
                                }
                            } finally {
                                if (Bridge.is($t2, System.IDisposable)) {
                                    $t2.System$IDisposable$Dispose();
                                }
                            }
                            this.Fields.add(optionField);

                        } else if (Bridge.referenceEquals(rawField.Kind, BridgeDTO.Form.DynamicForm.FormFieldType.TextField)) {
                            this.Fields.add(($t3 = new BridgeDTO.Form.DynamicForm.TextFormField(), $t3.Id = rawField.Id, $t3.Label = rawField.Label, $t3));
                        }
                        var listPermission = (rawField.ListPermissions == null) ? new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.FormPermission)).ctor() : rawField.ListPermissions;
                        this.Fields.getItem(count).ListPermissions = listPermission;
                        count = (count + 1) | 0;
                    }
                } finally {
                    if (Bridge.is($t, System.IDisposable)) {
                        $t.System$IDisposable$Dispose();
                    }
                }
            },
            $ctor1: function (dynamicForm) {
                var $t, $t1, $t2, $t3;
                BridgeDTO.Form.DynamicForm.DynamicForm.ctor.call(this);
                /* 
                foreach (FormField ff in dynamicForm.Fields)
                {
                   if (ff.Kind == FormFieldType.RadioList)
                   {
                       RadioFormField rf = (RadioFormField)ff;
                       this.Fields.Add(new RadioFormField()
                       {
                           Id = ff.Id,
                           Label = ff.Label,
                           Options = (List<Option>)rf.Options
                       });
                   }
                   else if (ff.Kind == FormFieldType.TextField)
                   {
                       TextFormField tf = (TextFormField)ff;
                       this.Fields.Add(new TextFormField()
                       {
                           Id = tf.Id,
                           Label = tf.Label,
                           Required = tf.Required
                       });
                   }
                }
                // end
                */
                var rawFields = dynamicForm.Fields;
                this.Fields = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.FormField)).ctor();
                var count = 0;
                $t = Bridge.getEnumerator(rawFields);
                try {
                    while ($t.moveNext()) {
                        var rawField = $t.Current;

                        if (Bridge.referenceEquals(rawField.Kind, BridgeDTO.Form.DynamicForm.FormFieldType.RadioList)) {
                            var radioField = ($t1 = new BridgeDTO.Form.DynamicForm.RadioFormField(), $t1.Id = rawField.Id, $t1.Label = rawField.Label, $t1);
                            var listOption = rawField.Options;

                            radioField.Options = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.Option)).ctor();
                            $t1 = Bridge.getEnumerator(listOption);
                            try {
                                while ($t1.moveNext()) {
                                    var option = $t1.Current;
                                    radioField.Options.add(($t2 = new BridgeDTO.Form.DynamicForm.Option(), $t2.Value = option.Value, $t2.Text = option.Text, $t2));
                                }
                            } finally {
                                if (Bridge.is($t1, System.IDisposable)) {
                                    $t1.System$IDisposable$Dispose();
                                }
                            }
                            this.Fields.add(radioField);

                        } else if (Bridge.referenceEquals(rawField.Kind, BridgeDTO.Form.DynamicForm.FormFieldType.OptionList)) {
                            var optionField = ($t2 = new BridgeDTO.Form.DynamicForm.OptionsFormField(), $t2.Id = rawField.Id, $t2.Label = rawField.Label, $t2);
                            var listOption1 = rawField.Options;

                            optionField.Options = new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.Option)).ctor();
                            $t2 = Bridge.getEnumerator(listOption1);
                            try {
                                while ($t2.moveNext()) {
                                    var option1 = $t2.Current;
                                    optionField.Options.add(($t3 = new BridgeDTO.Form.DynamicForm.Option(), $t3.Value = option1.Value, $t3.Text = option1.Text, $t3));
                                }
                            } finally {
                                if (Bridge.is($t2, System.IDisposable)) {
                                    $t2.System$IDisposable$Dispose();
                                }
                            }
                            this.Fields.add(optionField);

                        } else if (Bridge.referenceEquals(rawField.Kind, BridgeDTO.Form.DynamicForm.FormFieldType.TextField)) {
                            this.Fields.add(($t3 = new BridgeDTO.Form.DynamicForm.TextFormField(), $t3.Id = rawField.Id, $t3.Label = rawField.Label, $t3.Required = rawField.Required, $t3));
                        }
                        var listPermission = (rawField.ListPermission == null) ? new (System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.FormPermission)).ctor() : rawField.ListPermission;
                        this.Fields.getItem(count).ListPermissions = listPermission;
                        count = (count + 1) | 0;
                    }
                } finally {
                    if (Bridge.is($t, System.IDisposable)) {
                        $t.System$IDisposable$Dispose();
                    }
                }
            }
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.FormAjaxParams", {
        fields: {
            Url: null,
            FormId: null,
            UrlAfterPost: null,
            FormTag: null
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.FormField", {
        fields: {
            Kind: 0,
            Id: null,
            Label: null,
            TagId: null,
            PlaceHolder: null,
            ListPermissions: null
        },
        methods: {
            IsPermissionPresent: function (str) {
                return (System.Linq.Enumerable.from(this.ListPermissions).firstOrDefault(function (n) {
                        return System.String.equals(n.Key, str);
                    }, null) != null);
            },
            GetStrPermissionValue: function (str) {
                var fp = System.Linq.Enumerable.from(this.ListPermissions).firstOrDefault(function (n) {
                        return System.String.equals(n.Key, str);
                    }, null);
                return (fp != null ? fp.Value : "");
            },
            GetBoolPermissionValue: function (str) {
                var fp = System.Linq.Enumerable.from(this.ListPermissions).firstOrDefault(function (n) {
                        return System.String.equals(n.Key, str);
                    }, null);
                return (fp != null ? true : false);
            },
            IsPermissionSubStringPresent: function (str) {
                return (System.Linq.Enumerable.from(this.ListPermissions).firstOrDefault(function (n) {
                        return System.String.contains(n.Key,str);
                    }, null) != null);
            }
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.FormFieldType", {
        $kind: "enum",
        statics: {
            fields: {
                TextField: 1,
                TextArea: 2,
                RadioList: 3,
                OptionList: 4,
                MultiselectOptionList: 5,
                Checkbox: 6,
                Calender: 7
            }
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.FormPermission", {
        fields: {
            Key: null,
            Value: null
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.Option", {
        fields: {
            Value: null,
            Text: null
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.ValidateMode", {
        fields: {
            ValidationName: null,
            ValidationErrorMessage: null
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.OptionsFormField", {
        inherits: [BridgeDTO.Form.DynamicForm.FormField],
        fields: {
            Options: null
        },
        ctors: {
            ctor: function () {
                this.$initialize();
                BridgeDTO.Form.DynamicForm.FormField.ctor.call(this);
                this.Kind = BridgeDTO.Form.DynamicForm.FormFieldType.OptionList;
            }
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.RadioFormField", {
        inherits: [BridgeDTO.Form.DynamicForm.FormField],
        fields: {
            Options: null
        },
        ctors: {
            ctor: function () {
                this.$initialize();
                BridgeDTO.Form.DynamicForm.FormField.ctor.call(this);
                this.Kind = BridgeDTO.Form.DynamicForm.FormFieldType.RadioList;
            }
        }
    });

    Bridge.define("BridgeDTO.Form.DynamicForm.TextFormField", {
        inherits: [BridgeDTO.Form.DynamicForm.FormField],
        fields: {
            Required: false
        },
        ctors: {
            ctor: function () {
                this.$initialize();
                BridgeDTO.Form.DynamicForm.FormField.ctor.call(this);
                this.Kind = BridgeDTO.Form.DynamicForm.FormFieldType.TextField;
                this.Required = false;
            }
        }
    });
});
