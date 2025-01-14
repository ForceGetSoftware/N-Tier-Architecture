﻿using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.FilterExpressionCreator.Filters;

namespace N_Tier.Shared.N_Tier.Core.Common;

[EntityFilterSet]
public class GetAllRequest<TEntity>
{
    public EntityFilter<TEntity> Filter { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public string Search { get; set; }
    public string OrderBy { get; set; }
}
