﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace pwr_msi.Models.Enum {
    public enum OrderTaskType {
        CREATE,
        PAY,
        ACCEPT,
        REJECT,
        PREPARE,
        DELIVER,
        COMPLETE,
        CANCEL,
    }

    public enum OrderTaskPerformer {
        CUSTOMER,
        ACCEPTOR,
        DELIVERY,
        SYSTEM,
    }

    public static class OrderTaskTypeSettings {
        public readonly static ImmutableDictionary<OrderTaskType, List<OrderTaskType>> allowedTransitions =
            new Dictionary<OrderTaskType, List<OrderTaskType>>() {
                {OrderTaskType.CREATE, new List<OrderTaskType> {OrderTaskType.PAY, OrderTaskType.CANCEL}},
                {OrderTaskType.PAY, new List<OrderTaskType> {OrderTaskType.ACCEPT, OrderTaskType.REJECT}},
                {OrderTaskType.ACCEPT, new List<OrderTaskType> {OrderTaskType.PREPARE}},
                {OrderTaskType.PREPARE, new List<OrderTaskType> {OrderTaskType.DELIVER}},
                {OrderTaskType.DELIVER, new List<OrderTaskType> {OrderTaskType.COMPLETE}},
            }.ToImmutableDictionary();

        public readonly static ImmutableDictionary<OrderTaskType, List<OrderTaskPerformer>> allowedPerformers = new Dictionary<OrderTaskType, List<OrderTaskPerformer>> {
            {OrderTaskType.CREATE, new List<OrderTaskPerformer> {OrderTaskPerformer.CUSTOMER}},
            {OrderTaskType.PAY, new List<OrderTaskPerformer> {OrderTaskPerformer.CUSTOMER}},
            {OrderTaskType.ACCEPT, new List<OrderTaskPerformer> {OrderTaskPerformer.ACCEPTOR}},
            {OrderTaskType.REJECT, new List<OrderTaskPerformer> {OrderTaskPerformer.ACCEPTOR}},
            {OrderTaskType.PREPARE, new List<OrderTaskPerformer> {OrderTaskPerformer.ACCEPTOR}},
            {OrderTaskType.DELIVER, new List<OrderTaskPerformer> {OrderTaskPerformer.DELIVERY}},
            {OrderTaskType.COMPLETE, new List<OrderTaskPerformer> {OrderTaskPerformer.SYSTEM}},
            {OrderTaskType.CANCEL, new List<OrderTaskPerformer> {OrderTaskPerformer.CUSTOMER, OrderTaskPerformer.SYSTEM}},
        }.ToImmutableDictionary();

        public readonly static ImmutableDictionary<OrderTaskType, OrderStatus> statusByTaskType = new Dictionary<OrderTaskType, OrderStatus> {
            {OrderTaskType.CREATE, OrderStatus.CREATED},
            {OrderTaskType.PAY, OrderStatus.PAID},
            {OrderTaskType.ACCEPT, OrderStatus.ACCEPTED},
            {OrderTaskType.REJECT, OrderStatus.REJECTED},
            {OrderTaskType.PREPARE, OrderStatus.PREPARED},
            {OrderTaskType.DELIVER, OrderStatus.DELIVERED},
            {OrderTaskType.COMPLETE, OrderStatus.COMPLETED},
            {OrderTaskType.CANCEL, OrderStatus.CANCELLED},
        }.ToImmutableDictionary();

        public readonly static ImmutableDictionary<OrderStatus, OrderTaskType> taskTypeByStatus =
            statusByTaskType.Keys.ToImmutableDictionary(k => statusByTaskType[k], k => k);
    }
}
