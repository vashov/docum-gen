<template>
    <div>
        <input type="file" style="display: none" ref="inputFile" accept=".html" @change="handleFileChange" />
        <button :disabled="this.loading" class="btn btn-success mx-1" v-on:click="chooseFile">Upload file</button>
        <button :disabled="this.loading" class="btn btn-primary mx-1" v-on:click="fetchData">Refresh</button>
    </div>

    <div class="m-5">
        <div class="content">
            <ul class="pagination justify-content-center">
                <li :class="{ disabled: currentPage===1 }" class="page-item" v-on:click="changePage(-1)">
                    <button class="page-link" tabindex="-1">Previous</button>
                </li>
                <li class="page-item"><a class="page-link disabled">{{ currentPage }} / {{ totalPages }}</a></li>
                <li :class="{ disabled: currentPage===totalPages }" class="page-item" v-on:click="changePage(1)">
                    <button class="page-link">Next</button>
                </li>
            </ul>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th scope="col" class="col-1">File name</th>
                        <th scope="col" class="col-1">Status</th>
                        <th scope="col" class="col-1"></th>
                    </tr>
                </thead>
                <tbody>
                    <template v-if="page">
                        <tr v-for="item in page.data" :key="item.fileOrderId">
                            <td scope="row">{{ item.fileNameSource }}</td>
                            <td>{{ getStatus(item.status) }}</td>
                            <td>
                                <button type="button"
                                        v-on:click="downloadFileResult(item.fileOrderId)"
                                        :disabled="!canBeDownloaded(item.status)"
                                        class="btn btn-outline-primary btn-sm mx-1">
                                    Download
                                </button>
                                <button type="button"
                                        v-on:click="deleteFileOrder(item.fileOrderId)"
                                        :disabled="!canBeDeleted(item.status)"
                                        class="btn btn-danger btn-sm mx-1">
                                    X
                                </button>
                            </td>
                        </tr>
                    </template>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script lang="ts">
    import { defineComponent } from 'vue';
    import { FileOrder } from 'services/fileOrders/models/fileOrder';
    import { PageResult } from 'services/fileOrders/models/pageResult';
    import * as FileOrderService from '../services/fileOrders/fileOrderService';
    import { FileOrderGetListRequest } from 'services/fileOrders/requests/fileOrderGetListRequest';
    import { getStatusDescription, FileOrderStatus } from '../services/fileOrders/models/fileOrderStatus';

    interface Data {
        loading: boolean,
        currentPage: number,
        page: null | PageResult<FileOrder>,
    }

    export default defineComponent({
        data(): Data {
            return {
                loading: false,
                currentPage: 1,
                page: null,
            };
        },
        computed: {
            totalPages(): number {
                return Math.max(1, this.page?.totalPages ?? 1);
            }
        },
        created() {
            this.fetchData();
        },
        watch: {
            currentPage: 'fetchData'
        },
        methods: {
            getStatus(status: FileOrderStatus): string {
                return getStatusDescription(status);
            },
            canBeDeleted(status: FileOrderStatus): boolean {
                if (this.loading)
                    return false;
                return status !== FileOrderStatus.FilesDeleted;
            },
            canBeDownloaded(status: FileOrderStatus): boolean {
                if (this.loading)
                    return false;
                return status === FileOrderStatus.Processed;
            },
            changePage(value: number) {
                const tempPage = this.currentPage + value;
                if (tempPage > 0 && tempPage <= this.totalPages) {
                    this.currentPage = tempPage;
                }
            },
            beforeRequest() {
                this.loading = true;
            },
            afterRequest() {
                this.loading = false;
            },
            deleteFileOrder(fileOrderId: string) {
                this.beforeRequest();

                FileOrderService.deleteFilesFromOrder(fileOrderId)
                    .then(this.fetchData)
                    .finally(this.afterRequest);
            },
            downloadFileResult(fileOrderId: string) {
                this.beforeRequest();

                FileOrderService.downloadFileResult(fileOrderId)
                    .finally(this.afterRequest);
            },
            fetchData(): void {
                this.beforeRequest();
                const defaultPageSize = 5;

                const listRequest: FileOrderGetListRequest = {
                    pageNumber: this.currentPage,
                    pageSize: this.page?.pageSize ?? defaultPageSize
                }

                FileOrderService.getFileOrderList(listRequest)
                    .then(page => {
                        if (page === null) {
                            page = {
                                pageSize: this.page?.pageSize ?? defaultPageSize,
                                pageNumber: this.currentPage,
                                totalPages: this.totalPages,
                                totalItems: this.page?.totalItems ?? 0,
                                data: []
                            }
                        }
                        this.page = page;
                    })
                    .finally(this.afterRequest);
            },
            handleFileChange(event: Event) {
                const target = event.target as HTMLInputElement;
                const files = target.files;

                if (files && files.length > 0) {
                    this.beforeRequest();

                    const fileToUpload: File = files[0];

                    const inputFile = this.$refs.inputFile as HTMLInputElement;
                    inputFile.value = '';

                    FileOrderService.uploadFileSource(fileToUpload)
                        .then(this.fetchData)
                        .catch(this.fetchData)
                        .finally(this.afterRequest);
                }
            },
            chooseFile() {
                const inputFile = this.$refs.inputFile as HTMLInputElement;
                inputFile.click();
            },
        },
    });
</script>